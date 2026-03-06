using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.Core.Utilities.Security.JWT;
using wixi.Entities.Concrete;
using wixi.Entities.Concrete.Identity;
using wixi.Entities.DTOs;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using wixi.Entities.Concrete.Client;
using wixi.Entities.Concrete.Application;

namespace wixi.Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenHelper _tokenHelper;
        private readonly WixiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientService _clientService;
        private readonly IApplicationService _applicationService;

        private readonly ILogger<AuthManager> _logger;

        public AuthManager(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ITokenHelper tokenHelper,
            WixiDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IClientService clientService,
            IApplicationService applicationService,
            ILogger<AuthManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenHelper = tokenHelper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _clientService = clientService;
            _applicationService = applicationService;
            _logger = logger;
        }

        public async Task<TokenDto> LoginAsync(UserForLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new Exception("Parola hatalı");

            var token = await _tokenHelper.CreateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            return token;
        }

        public async Task<TokenDto> RegisterAsync(UserForRegisterDto registerDto)
        {
            _logger.LogInformation("Registration started for email: {Email}, ClientCode: {ClientCode}", 
                registerDto.Email, registerDto.ClientCode ?? "N/A");

            // If client code is provided, validate it
            PendingClientCode? pendingCode = null;
            if (!string.IsNullOrWhiteSpace(registerDto.ClientCode))
            {
                // Trim and normalize the client code
                var normalizedClientCode = registerDto.ClientCode.Trim();
                _logger.LogInformation("Validating pending client code: {ClientCode}", normalizedClientCode);
                
                // Use explicit IsUsed == false check (SQL Server bit field)
                pendingCode = await _context.PendingClientCodes
                    .FirstOrDefaultAsync(p => p.ClientCode == normalizedClientCode && p.IsUsed == false);

                if (pendingCode == null)
                {
                    _logger.LogWarning("Pending client code not found or already used: {ClientCode}", normalizedClientCode);
                    throw new Exception("Geçersiz veya kullanılmış müşteri kodu");
                }

                _logger.LogInformation("Pending client code found - ID: {Id}, Email: {Email}, ExpirationDate: {ExpirationDate}", 
                    pendingCode.Id, pendingCode.Email, pendingCode.ExpirationDate);

                if (pendingCode.ExpirationDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Pending client code expired: {ClientCode}, ExpirationDate: {ExpirationDate}", 
                        normalizedClientCode, pendingCode.ExpirationDate);
                    throw new Exception("Müşteri kodunun süresi dolmuş");
                }

                // Email must match the pending code email
                if (!string.IsNullOrEmpty(pendingCode.Email) && 
                    !pendingCode.Email.Equals(registerDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Email mismatch - PendingCode.Email: {PendingEmail}, RegisterDto.Email: {RegisterEmail}", 
                        pendingCode.Email, registerDto.Email);
                    throw new Exception("Müşteri kodu bu email adresi için geçerli değil");
                }

                _logger.LogInformation("Pending client code validated successfully");
            }

            _logger.LogInformation("Creating user for email: {Email}", registerDto.Email);

            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                _logger.LogError("User creation failed for email: {Email}, Errors: {Errors}", registerDto.Email, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("User created successfully - UserId: {UserId}, Email: {Email}", user.Id, user.Email);

            // If valid client code was provided, create client profile and mark code as used
            if (pendingCode != null)
            {
                _logger.LogInformation("Starting transaction for client profile creation - UserId: {UserId}, PendingCodeId: {PendingCodeId}", 
                    user.Id, pendingCode.Id);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("Transaction started successfully");

                    // Assign "Danisan" role (ID: 3) to the user
                    const string clientRoleName = "Danisan";
                    _logger.LogInformation("Checking if role exists: {RoleName}", clientRoleName);
                    
                    if (!await _roleManager.RoleExistsAsync(clientRoleName))
                    {
                        _logger.LogInformation("Role does not exist, creating: {RoleName}", clientRoleName);
                        await _roleManager.CreateAsync(new AppRole { Name = clientRoleName });
                        _logger.LogInformation("Role created successfully: {RoleName}", clientRoleName);
                    }
                    
                    _logger.LogInformation("Adding user to role - UserId: {UserId}, Role: {RoleName}", user.Id, clientRoleName);
                    var roleResult = await _userManager.AddToRoleAsync(user, clientRoleName);
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogError("Role assignment failed - UserId: {UserId}, Role: {RoleName}, Errors: {Errors}", 
                            user.Id, clientRoleName, roleErrors);
                        throw new Exception($"Rol atanamadı: {roleErrors}");
                    }
                    
                    _logger.LogInformation("Role assigned successfully - UserId: {UserId}, Role: {RoleName}", user.Id, clientRoleName);
                    
                    // Ensure role assignment is saved before proceeding
                    _logger.LogInformation("Saving role assignment to database");
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Role assignment saved successfully");

                    // Create client profile - use pending code's ClientCode directly
                    _logger.LogInformation("Creating client profile - UserId: {UserId}, ClientCode: {ClientCode}", 
                        user.Id, pendingCode.ClientCode);

                    var clientCreateDto = new ClientCreateDto
                    {
                        UserId = user.Id,
                        FirstName = registerDto.FirstName,
                        LastName = registerDto.LastName,
                        Email = registerDto.Email,
                        Phone = registerDto.Phone ?? string.Empty,
                        DateOfBirth = registerDto.DateOfBirth,
                        Address = registerDto.Address,
                        Nationality = registerDto.Nationality,
                        EducationTypeId = registerDto.EducationTypeId,
                        ClientCode = pendingCode.ClientCode // Use pending code's ClientCode directly
                    };

                    _logger.LogInformation("Calling ClientService.CreateClientAsync");
                    var clientDto = await _clientService.CreateClientAsync(clientCreateDto);
                    _logger.LogInformation("ClientService.CreateClientAsync completed");
                    
                    // Save all changes (client with correct code, role assignment, etc.) within the transaction
                    _logger.LogInformation("Saving client to database within transaction");
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Client saved to database successfully");
                    
                    // Reload client to get the generated ID
                    _logger.LogInformation("Reloading client entity - UserId: {UserId}", user.Id);
                    var clientEntity = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id && c.DeletedAt == null);
                    if (clientEntity == null)
                    {
                        _logger.LogError("Client entity not found after save - UserId: {UserId}", user.Id);
                        throw new Exception("Client entity was not saved properly");
                    }
                    _logger.LogInformation("Client entity reloaded - ClientId: {ClientId}", clientEntity.Id);

                    // Add education history if provided
                    if (registerDto.EducationHistory != null && registerDto.EducationHistory.Any())
                    {
                        _logger.LogInformation("Adding education history - Count: {Count}, ClientId: {ClientId}", 
                            registerDto.EducationHistory.Count(), clientEntity.Id);
                        
                        foreach (var educationInfo in registerDto.EducationHistory)
                        {
                            try
                            {
                                educationInfo.ClientId = clientEntity.Id;
                                _logger.LogInformation("Adding education info - ClientId: {ClientId}, Degree: {Degree}", 
                                    clientEntity.Id, educationInfo.Degree);
                                await _clientService.AddEducationInfoAsync(educationInfo);
                                _logger.LogInformation("Education info added successfully");
                            }
                            catch (Exception ex)
                            {
                                // Log error but don't fail registration
                                // Education info can be added later
                                _logger.LogWarning(ex, "Failed to add education info - ClientId: {ClientId}, Error: {Error}", 
                                    clientEntity.Id, ex.Message);
                            }
                        }
                    }

                    // Mark pending code as used
                    _logger.LogInformation("Marking pending code as used - PendingCodeId: {PendingCodeId}", pendingCode.Id);
                    pendingCode.IsUsed = true;
                    pendingCode.UsedAt = DateTime.UtcNow;
                    
                    _logger.LogInformation("Saving pending code update to database");
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Pending code updated successfully");
                    
                    _logger.LogInformation("Committing transaction");
                    await transaction.CommitAsync();
                    _logger.LogInformation("Transaction committed successfully - UserId: {UserId}, ClientId: {ClientId}", 
                        user.Id, clientEntity.Id);
                    
                    // Create application AFTER transaction commit to avoid transaction conflicts
                    // This is done outside the transaction to ensure it doesn't interfere with the main registration flow
                    _logger.LogInformation("Attempting to create application after registration - ClientId: {ClientId}", clientEntity.Id);
                    try
                    {
                        var recognitionTemplate = await _context.ApplicationTemplates
                            .Where(t => t.IsActive && t.Type == ApplicationType.Recognition)
                            .FirstOrDefaultAsync();

                        if (recognitionTemplate != null)
                        {
                            _logger.LogInformation("Recognition template found - TemplateId: {TemplateId}, Name: {Name}", 
                                recognitionTemplate.Id, recognitionTemplate.Name);

                            var existingApp = await _context.Applications
                                .FirstOrDefaultAsync(a => a.ClientId == clientEntity.Id && a.ApplicationTemplateId == recognitionTemplate.Id);

                            if (existingApp == null)
                            {
                                _logger.LogInformation("Creating application - ClientId: {ClientId}, TemplateId: {TemplateId}", 
                                    clientEntity.Id, recognitionTemplate.Id);
                                
                                await _applicationService.CreateApplicationAsync(new ApplicationCreateDto
                                {
                                    ClientId = clientEntity.Id,
                                    TemplateId = recognitionTemplate.Id,
                                    Notes = $"Otomatik oluşturuldu - {recognitionTemplate.Name}"
                                });
                                
                                _logger.LogInformation("Application created successfully - ClientId: {ClientId}", clientEntity.Id);
                            }
                            else
                            {
                                _logger.LogInformation("Application already exists - ClientId: {ClientId}, ApplicationId: {ApplicationId}", 
                                    clientEntity.Id, existingApp.Id);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Recognition template not found - cannot create application automatically");
                        }
                    }
                    catch (Exception appEx)
                    {
                        // Log but don't fail registration - application can be created later
                        _logger.LogWarning(appEx, "Failed to create application after registration - ClientId: {ClientId}, Error: {Error}", 
                            clientEntity.Id, appEx.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during client registration transaction - UserId: {UserId}, Email: {Email}", 
                        user.Id, user.Email);

                    // Log inner exception details for debugging
                    var innerEx = ex.InnerException;
                    var errorDetails = ex.Message;
                    var fullStackTrace = ex.StackTrace;
                    
                    if (innerEx != null)
                    {
                        errorDetails += $" | Inner Exception: {innerEx.Message}";
                        _logger.LogError(innerEx, "Inner exception details - Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}", 
                            innerEx.GetType().Name, innerEx.Message, innerEx.StackTrace);
                        
                        if (innerEx.InnerException != null)
                        {
                            errorDetails += $" | Inner Inner: {innerEx.InnerException.Message}";
                            _logger.LogError(innerEx.InnerException, "Inner inner exception details - Type: {ExceptionType}, Message: {Message}", 
                                innerEx.InnerException.GetType().Name, innerEx.InnerException.Message);
                        }
                    }
                    
                    _logger.LogError("Full exception stack trace: {StackTrace}", fullStackTrace);

                    _logger.LogInformation("Rolling back transaction");
                    await transaction.RollbackAsync();
                    _logger.LogInformation("Transaction rolled back successfully");
                    
                    // Transaction rollback will undo all changes including client creation
                    // But we need to clean up the user that was created outside the transaction
                    // Use SQL to avoid EF relationship issues
                    _logger.LogInformation("Cleaning up user after transaction rollback - UserId: {UserId}", user.Id);
                    try
                    {
                        var userId = user.Id;
                        
                        // Check if client was created (it shouldn't be after rollback, but check anyway)
                        var createdClient = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId && c.DeletedAt == null);
                        if (createdClient != null)
                        {
                            _logger.LogWarning("Client entity found after rollback - ClientId: {ClientId}, UserId: {UserId}. Deleting...", 
                                createdClient.Id, userId);
                            // Client was created outside transaction, delete it first
                            _context.Clients.Remove(createdClient);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Client entity deleted successfully");
                        }
                        
                        _logger.LogInformation("Deleting user roles - UserId: {UserId}", userId);
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = {0}", userId);
                        
                        _logger.LogInformation("Deleting user claims - UserId: {UserId}", userId);
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserClaims] WHERE [UserId] = {0}", userId);
                        
                        _logger.LogInformation("Deleting user logins - UserId: {UserId}", userId);
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserLogins] WHERE [UserId] = {0}", userId);
                        
                        _logger.LogInformation("Deleting user tokens - UserId: {UserId}", userId);
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserTokens] WHERE [UserId] = {0}", userId);
                        
                        _logger.LogInformation("Deleting user - UserId: {UserId}", userId);
                        await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = {0}", userId);
                        
                        _logger.LogInformation("User cleanup completed successfully - UserId: {UserId}", userId);
                    }
                    catch (Exception deleteEx)
                    {
                        // Log but don't throw - user might already be deleted or client might not exist
                        _logger.LogError(deleteEx, "Error cleaning up during rollback - UserId: {UserId}, Error: {Error}", 
                            user.Id, deleteEx.Message);
                    }
                    
                    // Return a more user-friendly error message with inner exception details
                    throw new Exception($"Müşteri profili oluşturulamadı: {errorDetails}");
                }
            }

            var token = await _tokenHelper.CreateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            return token;
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            // Blacklist kontrolü
            var isBlacklisted = await _context.TokenBlacklist
                .AnyAsync(t => t.Token == refreshToken && t.ExpirationDate > DateTime.UtcNow);

            if (isBlacklisted)
                throw new Exception("Geçersiz veya süresi dolmuş refresh token");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => 
                u.RefreshToken == refreshToken && u.RefreshTokenEndDate > DateTime.UtcNow);

            if (user == null)
                throw new Exception("Geçersiz refresh token");

            var token = await _tokenHelper.CreateToken(user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            return token;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => 
                u.RefreshToken == refreshToken);

            if (user != null)
            {
                // Access token'ı al
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(accessToken);
                        var expirationDate = jwtToken.ValidTo;

                        var token = await _context.TokenBlacklist.FirstOrDefaultAsync(t => t.Token == accessToken);
                        
                        if (token == null)
                        {
                            _context.TokenBlacklist.Add(new TokenBlacklist
                            {
                                Token = accessToken,
                                BlacklistedAt = DateTime.UtcNow,
                                ExpirationDate = expirationDate
                            });
                        }
                    }
                    catch (Exception)
                    {
                        // Token işleme hatası
                    }
                }

                // Refresh token'ı da blacklist'e ekle
                if (!string.IsNullOrEmpty(user.RefreshToken) && user.RefreshTokenEndDate.HasValue)
                {
                    var refreshTokenInBlacklist = await _context.TokenBlacklist.FirstOrDefaultAsync(t => t.Token == user.RefreshToken);
                    
                    if (refreshTokenInBlacklist == null)
                    {
                        _context.TokenBlacklist.Add(new TokenBlacklist
                        {
                            Token = user.RefreshToken,
                            BlacklistedAt = DateTime.UtcNow,
                            ExpirationDate = user.RefreshTokenEndDate.Value
                        });
                    }
                }

                user.RefreshToken = null;
                user.RefreshTokenEndDate = null;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı");

            return user;
        }
    }
}

