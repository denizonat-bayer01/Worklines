using Microsoft.AspNetCore.HttpOverrides;
using wixi.Core.Utilities.Security.Protection;
using Serilog;
using wixi.WebAPI.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using wixi.WebAPI.Extensions;

// Configure Serilog Bootstrap Logger
SerilogExtensions.ConfigureBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add DbContext (from Extensions)
    builder.Services.AddDbContextExt(builder.Configuration);

    // Add Identity & JWT (from Extensions)
    builder.Services.AddIdentityExt();
    builder.Services.AddJwtExt(builder.Configuration);

    // Add Business Services (from Extensions)
    builder.Services.AddServiceCollectionExt();
    // Secret protector for encrypting SMTP credentials
    builder.Services.AddSingleton<ISecretProtector, CompositeSecretProtector>();

    // Add Serilog (from Extensions)
    builder.AddSerilogExt();

    // Add Controllers
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<wixi.WebAPI.Filters.ModelValidationFilter>();
    });

    // Configure Forwarded Headers for Docker/Reverse Proxy (Real Client IP)
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                                   ForwardedHeaders.XForwardedProto | 
                                   ForwardedHeaders.XForwardedHost;
        
        // Allow any proxy (important for Docker + Nginx/Apache)
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
        
        // Set forward limit (number of proxies to trust)
        options.ForwardLimit = 10; // Increased to handle multiple proxies
        
        // Explicitly set header names (in case reverse proxy uses different names)
        options.ForwardedForHeaderName = "X-Forwarded-For";
        options.ForwardedHostHeaderName = "X-Forwarded-Host";
        options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
    });
    
    // Smart IP Resolver - Extract real client IP from various headers
    builder.Services.AddSingleton<Func<HttpContext, string>>(serviceProvider => 
    {
        return (HttpContext context) =>
        {
            // Priority order: Check multiple header sources
            var headerPriority = new[]
            {
                "CF-Connecting-IP",      // Cloudflare
                "True-Client-IP",        // Akamai, Cloudflare Enterprise
                "X-Real-IP",             // Nginx
                "X-Forwarded-For",       // Standard proxy header
                "X-Client-IP",           // Apache
                "X-Original-For",        // Custom proxy
                "Forwarded",             // RFC 7239
                "X-Cluster-Client-IP"    // Rackspace LB
            };

            foreach (var header in headerPriority)
            {
                var value = context.Request.Headers[header].FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    // X-Forwarded-For can contain multiple IPs (client, proxy1, proxy2)
                    // Take the first one (real client)
                    var ip = value.Split(',')[0].Trim();
                    
                    // Validate it's not internal/private IP
                    if (!ip.StartsWith("127.") && 
                        !ip.StartsWith("10.") && 
                        !ip.StartsWith("172.16.") && 
                        !ip.StartsWith("172.17.") && 
                        !ip.StartsWith("172.18.") && 
                        !ip.StartsWith("172.19.") && 
                        !ip.StartsWith("192.168.") &&
                        !ip.Contains("::ffff:127.") &&
                        !ip.Contains("::1"))
                    {
                        return ip;
                    }
                }
            }
            
            // Fallback to connection remote IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
        };
    });

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WixiCorsPolicy", policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Development: Allow all origins for testing
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                // Production: Allow specific origins
                policy.WithOrigins(
                        "https://worklines.de",
                        "https://www.worklines.de",
                        "https://api.worklines.de",
                        "https://worklines.wixisoftware.com",
                        "http://78.188.86.124:5500",
                        "https://78.188.86.124:5500"
                      )
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
    });

    // Add API Explorer & Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Wixi WorkLines API",
            Version = "v1",
            Description = "Wixi WorkLines API Documentation"
        });

        // JWT Bearer auth for Swagger (Authorize lock icon)
        var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter JWT as: Bearer {token}",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            { securityScheme, new string[] {} }
        });
    });

    var app = builder.Build();

    // Use Forwarded Headers (MUST be before Serilog to capture real IP)
    app.UseForwardedHeaders();

    // Use Serilog Request Logging (from Extensions)
    app.UseSerilogRequestLoggingExt();

    // Global exception handling (must be early in the pipeline)
    app.UseGlobalException();

    // Configure Swagger (available in all environments)
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wixi WorkLines API v1");
        options.RoutePrefix = "swagger"; // URL: /swagger/index.html
    });

    // Enable CORS (MUST be before Authentication)
    app.UseCors("WixiCorsPolicy");

    app.UseHttpsRedirection();

    // Enable static files (for wwwroot)
    app.UseStaticFiles();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // Map Controllers (AuthController, etc.)
    app.MapControllers();

    // Sample endpoint - WeatherForecast
    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

    // Seed initial data
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            await wixi.DataAccess.Extensions.SeedDataExtensions.SeedDocumentTrackingDataAsync(scope.ServiceProvider);
            Log.Information("Seed data completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding data");
        }
    }

    Log.Information("Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    SerilogExtensions.FlushAndCloseSerilog();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

