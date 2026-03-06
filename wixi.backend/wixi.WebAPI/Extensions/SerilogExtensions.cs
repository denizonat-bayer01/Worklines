using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using wixi.WebAPI.Enrichers;

namespace wixi.WebAPI.Extensions
{
    /// <summary>
    /// Serilog konfigürasyon ve entegrasyon extension metodları
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Serilog bootstrap logger'ı yapılandırır (uygulama başlangıcında çalışır)
        /// </summary>
        public static void ConfigureBootstrapLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("Starting Wixi WorkLines API...");
        }

        /// <summary>
        /// WebApplicationBuilder'a Serilog entegrasyonu ekler
        /// appsettings.json'dan konfigürasyonu okur ve Serilog'u host'a entegre eder
        /// Database logging desteği ile (ApplicationLogs tablosuna kaydeder)
        /// </summary>
        public static WebApplicationBuilder AddSerilogExt(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Wixi.WorkLines.API")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Destructure.ByTransforming<SmtpSettings>(s => new
                {
                    s.Id,
                    s.Host,
                    s.Port,
                    s.UseSsl,
                    s.UserName,
                    PasswordEnc = "***REDACTED***",
                    s.FromName,
                    s.FromEmail,
                    s.TimeoutMs,
                    s.RetryCount,
                    s.UpdatedAt,
                    s.UpdatedBy
                })
                .Destructure.ByTransformingWhere<object>(
                    obj => obj.GetType().GetProperties().Any(p => 
                        p.Name.Equals("Password", StringComparison.OrdinalIgnoreCase) || 
                        p.Name.Equals("PasswordEnc", StringComparison.OrdinalIgnoreCase)),
                    obj =>
                    {
                        var props = obj.GetType().GetProperties()
                            .Where(p => !p.Name.Equals("Password", StringComparison.OrdinalIgnoreCase) && 
                                       !p.Name.Equals("PasswordEnc", StringComparison.OrdinalIgnoreCase))
                            .ToDictionary(p => p.Name, p => p.GetValue(obj));
                        props["Password"] = "***REDACTED***";
                        return props;
                    })
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.MSSqlServer(
                    connectionString: connectionString!,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "ApplicationLogs",
                        AutoCreateSqlTable = true, // Tablo yoksa otomatik oluştur
                        BatchPostingLimit = 100, // Her 100 log'da bir database'e yaz
                        BatchPeriod = TimeSpan.FromSeconds(5), // Veya 5 saniyede bir
                        SchemaName = "dbo"
                    },
                    columnOptions: new ColumnOptions
                    {
                        // Standart kolonlar
                        TimeStamp = { ColumnName = "Timestamp", DataType = SqlDbType.DateTime2 },
                        Level = { ColumnName = "Level", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        Message = { ColumnName = "Message", DataType = SqlDbType.NVarChar, DataLength = 4000 },
                        Exception = { ColumnName = "Exception", DataType = SqlDbType.NVarChar, DataLength = -1 },
                        Properties = { ColumnName = "Properties", DataType = SqlDbType.NVarChar, DataLength = -1 },
                        
                    // Custom kolonlar (ApplicationLog entity ile eşleşecek)
                        AdditionalColumns = new Collection<SqlColumn>
                        {
                            new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                            new SqlColumn { ColumnName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 10, AllowNull = true },
                            new SqlColumn { ColumnName = "StatusCode", DataType = SqlDbType.Int, AllowNull = true },
                            new SqlColumn { ColumnName = "ElapsedMs", DataType = SqlDbType.Float, AllowNull = true },
                            new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                            new SqlColumn { ColumnName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = true },
                            new SqlColumn { ColumnName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = true },
                            new SqlColumn { ColumnName = "RemoteIP", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
                            new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                            new SqlColumn { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                            new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                            new SqlColumn { ColumnName = "Application", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = true },
                            new SqlColumn { ColumnName = "Environment", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true }
                        }
                        
                        // Note: StoreAsEnum property removed in Serilog.Sinks.MSSqlServer 9.x
                        // Level is now automatically stored as string by default
                    }));

            return builder;
        }

        /// <summary>
        /// WebApplication'a Serilog request logging middleware'ini ekler
        /// Her HTTP request otomatik olarak loglanır (method, path, status code, elapsed time)
        /// </summary>
        public static WebApplication UseSerilogRequestLoggingExt(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                // Request tamamlandığında loglanacak mesaj template'i
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                // Log level'i belirle (hatalara göre)
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode > 399
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;

                // Her request için otomatik mapping (RequestLogEnricher ile merkezi mapping)
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // ApplicationLog entity kolonlarına otomatik mapping
                    RequestLogEnricher.EnrichDiagnosticContext(diagnosticContext, httpContext);
                    
                    // Note: {Elapsed} otomatik olarak Serilog tarafından eklenir
                    // ElapsedMs mapping için custom enricher veya message template kullanılabilir
                };
            });

            return app;
        }

        /// <summary>
        /// Serilog'u düzgün bir şekilde kapatır ve tüm logların yazılmasını garanti eder
        /// </summary>
        public static void FlushAndCloseSerilog()
        {
            Log.Information("Application stopped cleanly");
            Log.CloseAndFlush();
        }
    }
}

