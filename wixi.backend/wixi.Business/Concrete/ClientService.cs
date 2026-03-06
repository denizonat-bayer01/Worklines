using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.DTOs;
using wixi.Entities.Concrete.Application;
using wixi.Entities.Concrete.Identity;

namespace wixi.Business.Concrete
{
    public class ClientService : IClientService
    {
        private readonly WixiDbContext _context;
        private readonly ILogger<ClientService> _logger;
        private readonly IApplicationService _applicationService;
        private readonly UserManager<AppUser> _userManager;

        public ClientService(
            WixiDbContext context,
            ILogger<ClientService> logger,
            IApplicationService applicationService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _logger = logger;
            _applicationService = applicationService;
            _userManager = userManager;
        }

        public async Task<ClientResponseDto> CreateClientAsync(ClientCreateDto createDto)
        {
            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == createDto.UserId);
            if (!userExists)
            {
                throw new Exception("User not found");
            }

            // Check if client already exists for this user
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == createDto.UserId && c.DeletedAt == null);
            if (existingClient != null)
            {
                throw new Exception("Client profile already exists for this user");
            }

            // Validate EducationTypeId if provided
            if (createDto.EducationTypeId.HasValue)
            {
                var educationTypeExists = await _context.EducationTypes.AnyAsync(et => et.Id == createDto.EducationTypeId.Value);
                if (!educationTypeExists)
                {
                    throw new Exception($"EducationType with ID {createDto.EducationTypeId.Value} not found");
                }
            }

            // Generate ClientCode if not provided
            string clientCode;
            if (!string.IsNullOrWhiteSpace(createDto.ClientCode))
            {
                // Use provided ClientCode (e.g., from PendingClientCode)
                clientCode = createDto.ClientCode;
                
                // Verify it's unique - check if it's already used in Clients table
                // Note: We don't check PendingClientCodes because this code might be from a valid pending code
                // that is being used now. The AuthManager will handle marking it as used.
                var codeExistsInClients = await _context.Clients.AnyAsync(c => c.ClientCode == clientCode && c.DeletedAt == null);
                if (codeExistsInClients)
                {
                    throw new Exception($"ClientCode '{clientCode}' is already in use by another client");
                }
            }
            else
            {
                // Generate unique ClientCode
                bool isUnique = false;
                int attempts = 0;
                const int maxAttempts = 10;
                
                do
                {
                    clientCode = $"CL-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}";
                    var codeExists = await _context.Clients.AnyAsync(c => c.ClientCode == clientCode && c.DeletedAt == null) ||
                                     await _context.PendingClientCodes.AnyAsync(p => p.ClientCode == clientCode);
                    isUnique = !codeExists;
                    attempts++;
                } while (!isUnique && attempts < maxAttempts);
                
                if (!isUnique)
                {
                    throw new Exception("Failed to generate unique ClientCode after multiple attempts");
                }
            }

            var client = new Entities.Concrete.Client.Client
            {
                UserId = createDto.UserId,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone ?? string.Empty,
                DateOfBirth = createDto.DateOfBirth,
                Nationality = createDto.Nationality,
                Address = createDto.Address,
                EducationTypeId = createDto.EducationTypeId,
                ClientCode = clientCode,
                RegistrationDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Clients.Add(client);
            // Don't call SaveChangesAsync here - let the calling method handle it within its transaction
            // This allows the caller to manage the transaction and rollback if needed
            
            _logger.LogInformation("Client profile entity added for user {UserId} (will be saved by caller)", createDto.UserId);

            // Return a DTO without saving - the caller will save and reload
            // We need to get the ID after save, so we'll return a temporary DTO
            // The caller should reload after SaveChangesAsync
            return new ClientResponseDto
            {
                Id = 0, // Will be set after save
                UserId = createDto.UserId,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone ?? string.Empty,
                DateOfBirth = createDto.DateOfBirth,
                Nationality = createDto.Nationality,
                Address = createDto.Address,
                ClientCode = client.ClientCode,
                RegistrationDate = client.RegistrationDate,
                Status = "Active",
                EducationTypeId = createDto.EducationTypeId,
                EducationHistory = new List<EducationInfoDto>()
            };
        }

        public async Task<ClientResponseDto> GetClientByIdAsync(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .Include(c => c.EducationType)
                .Include(c => c.EducationHistory)
                .FirstOrDefaultAsync(c => c.Id == clientId && c.DeletedAt == null);

            if (client == null)
            {
                throw new Exception("Client not found");
            }

            return MapToClientDto(client);
        }

        public async Task<ClientResponseDto> GetClientByUserIdAsync(int userId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .Include(c => c.EducationType)
                .Include(c => c.EducationHistory)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.DeletedAt == null);

            if (client == null)
            {
                throw new Exception("Client profile not found for this user");
            }

            return MapToClientDto(client);
        }

        public async Task<List<ClientResponseDto>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .Include(c => c.User)
                .Include(c => c.EducationType)
                .Include(c => c.EducationHistory)
                .Where(c => c.DeletedAt == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return clients.Select(c => MapToClientDto(c)).ToList();
        }

        public async Task<ClientResponseDto> UpdateClientAsync(int clientId, ClientUpdateDto updateDto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId && c.DeletedAt == null);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            client.FirstName = updateDto.FirstName;
            client.LastName = updateDto.LastName;
            client.Email = updateDto.Email;
            client.Phone = updateDto.Phone;
            client.DateOfBirth = updateDto.DateOfBirth;
            client.Nationality = updateDto.Nationality;
            client.Address = updateDto.Address;
            client.EducationTypeId = updateDto.EducationTypeId;
            client.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Client profile updated: {ClientId}", clientId);

            return await GetClientByIdAsync(clientId);
        }

        public async Task<bool> DeleteClientAsync(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId && c.DeletedAt == null);
            
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            var userId = client.UserId;
            var user = client.User;

            // Soft delete client
            client.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Client profile deleted (soft): {ClientId}", clientId);

            // Delete user and roles if user exists
            if (user != null)
            {
                try
                {
                    // Check if user is admin (don't delete admin users)
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin") || 
                                  await _userManager.IsInRoleAsync(user, "SuperAdmin");
                    
                    if (isAdmin)
                    {
                        _logger.LogWarning("Attempted to delete admin user {UserId} for client {ClientId}. User deletion skipped.", user.Id, clientId);
                    }
                    else
                    {
                        // Use raw SQL to delete all Identity-related data (more reliable than EF)
                        // Delete user roles
                        var userRolesDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = {0}", userId);
                        _logger.LogInformation("Deleted {Count} user roles for user {UserId}", userRolesDeleted, userId);
                        
                        // Delete user claims
                        var userClaimsDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserClaims] WHERE [UserId] = {0}", userId);
                        _logger.LogInformation("Deleted {Count} user claims for user {UserId}", userClaimsDeleted, userId);
                        
                        // Delete user logins
                        var userLoginsDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserLogins] WHERE [UserId] = {0}", userId);
                        _logger.LogInformation("Deleted {Count} user logins for user {UserId}", userLoginsDeleted, userId);
                        
                        // Delete user tokens
                        var userTokensDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserTokens] WHERE [UserId] = {0}", userId);
                        _logger.LogInformation("Deleted {Count} user tokens for user {UserId}", userTokensDeleted, userId);
                        
                        // Delete user preferences if exists
                        var userPreferencesDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[wixi_UserPreference] WHERE [UserId] = {0}", userId.ToString());
                        _logger.LogInformation("Deleted {Count} user preferences for user {UserId}", userPreferencesDeleted, userId);
                        
                        // Delete notifications if exists
                        var notificationsDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[wixi_Notification] WHERE [UserId] = {0}", userId);
                        _logger.LogInformation("Deleted {Count} notifications for user {UserId}", notificationsDeleted, userId);
                        
                        // Finally, delete the user itself using SQL (most reliable method)
                        var userDeleted = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = {0}", userId);
                        
                        if (userDeleted > 0)
                        {
                            _logger.LogInformation("User {UserId} and all roles deleted for client {ClientId}", userId, clientId);
                        }
                        else
                        {
                            _logger.LogWarning("User {UserId} was not deleted (may not exist)", userId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting user {UserId} for client {ClientId}. Exception: {Exception}", userId, clientId, ex.Message);
                    // Re-throw to ensure the caller knows about the failure
                    throw new Exception($"Müşteri silindi ancak kullanıcı silinirken hata oluştu: {ex.Message}", ex);
                }
            }

            return true;
        }

        public async Task<EducationInfoDto> AddEducationInfoAsync(EducationInfoCreateDto createDto)
        {
            // Validate client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == createDto.ClientId && c.DeletedAt == null);
            if (!clientExists)
            {
                throw new Exception("Client not found");
            }

            // Parse education level
            if (!Enum.TryParse<EducationLevel>(createDto.Level, true, out var educationLevel))
            {
                throw new Exception($"Invalid education level: {createDto.Level}");
            }

            var educationInfo = new Entities.Concrete.Client.EducationInfo
            {
                ClientId = createDto.ClientId,
                Level = educationLevel,
                Degree = createDto.Degree,
                Institution = createDto.Institution,
                FieldOfStudy = createDto.FieldOfStudy,
                StartDate = createDto.StartDate,
                GraduationDate = createDto.GraduationDate,
                Country = createDto.Country,
                IsCurrent = createDto.IsCurrent,
                Description = createDto.Description,
                GPA = createDto.GPA,
                CreatedAt = DateTime.UtcNow
            };

            _context.EducationInfos.Add(educationInfo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Education info added for client {ClientId}", createDto.ClientId);

            return MapToEducationInfoDto(educationInfo);
        }

        public async Task<EducationInfoDto> UpdateEducationInfoAsync(int educationInfoId, EducationInfoUpdateDto updateDto)
        {
            var educationInfo = await _context.EducationInfos.FindAsync(educationInfoId);
            if (educationInfo == null)
            {
                throw new Exception("Education info not found");
            }

            // Parse education level
            if (!Enum.TryParse<EducationLevel>(updateDto.Level, true, out var educationLevel))
            {
                throw new Exception($"Invalid education level: {updateDto.Level}");
            }

            educationInfo.Level = educationLevel;
            educationInfo.Degree = updateDto.Degree;
            educationInfo.Institution = updateDto.Institution;
            educationInfo.FieldOfStudy = updateDto.FieldOfStudy;
            educationInfo.StartDate = updateDto.StartDate;
            educationInfo.GraduationDate = updateDto.GraduationDate;
            educationInfo.Country = updateDto.Country;
            educationInfo.IsCurrent = updateDto.IsCurrent;
            educationInfo.Description = updateDto.Description;
            educationInfo.GPA = updateDto.GPA;
            educationInfo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Education info updated: {EducationInfoId}", educationInfoId);

            return MapToEducationInfoDto(educationInfo);
        }

        public async Task<bool> DeleteEducationInfoAsync(int educationInfoId)
        {
            var educationInfo = await _context.EducationInfos.FindAsync(educationInfoId);
            if (educationInfo == null)
            {
                throw new Exception("Education info not found");
            }

            _context.EducationInfos.Remove(educationInfo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Education info deleted: {EducationInfoId}", educationInfoId);
            return true;
        }

        public async Task<List<EducationTypeDto>> GetEducationTypesAsync()
        {
            var educationTypes = await _context.EducationTypes
                .Where(et => et.IsActive)
                .OrderBy(et => et.DisplayOrder)
                .ToListAsync();

            return educationTypes.Select(et => new EducationTypeDto
            {
                Id = et.Id,
                Code = et.Code,
                Name = et.Name,
                NameEn = et.NameEn,
                IsActive = et.IsActive,
                DisplayOrder = et.DisplayOrder
            }).ToList();
        }

        // Helper methods
        private EducationInfoDto MapToEducationInfoDto(Entities.Concrete.Client.EducationInfo educationInfo)
        {
            return new EducationInfoDto
            {
                Id = educationInfo.Id,
                ClientId = educationInfo.ClientId,
                Level = educationInfo.Level.ToString(),
                Degree = educationInfo.Degree,
                Institution = educationInfo.Institution,
                FieldOfStudy = educationInfo.FieldOfStudy,
                StartDate = educationInfo.StartDate,
                GraduationDate = educationInfo.GraduationDate,
                Country = educationInfo.Country,
                IsCurrent = educationInfo.IsCurrent,
                Description = educationInfo.Description,
                GPA = educationInfo.GPA
            };
        }

        private ClientResponseDto MapToClientDto(Entities.Concrete.Client.Client client)
        {
            int? age = null;
            if (client.DateOfBirth.HasValue)
            {
                var ageCalc = DateTime.UtcNow.Year - client.DateOfBirth.Value.Year;
                if (DateTime.UtcNow < client.DateOfBirth.Value.AddYears(ageCalc))
                {
                    ageCalc--;
                }
                age = ageCalc;
            }

            return new ClientResponseDto
            {
                Id = client.Id,
                UserId = client.UserId,
                UserEmail = client.User.Email ?? string.Empty,
                FirstName = client.FirstName,
                LastName = client.LastName,
                FullName = client.FullName,
                Email = client.Email,
                Phone = client.Phone,
                DateOfBirth = client.DateOfBirth,
                Age = age,
                Nationality = client.Nationality,
                Address = client.Address,
                ClientCode = client.ClientCode,
                RegistrationDate = client.RegistrationDate,
                Status = client.Status.ToString(),
                EducationTypeId = client.EducationTypeId,
                EducationTypeName = client.EducationType?.Name,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt,
                EducationHistory = client.EducationHistory.Select(e => MapToEducationInfoDto(e)).ToList()
            };
        }
    }
}
