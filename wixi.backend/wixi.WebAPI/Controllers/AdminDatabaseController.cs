using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete.Identity;
using Microsoft.AspNetCore.Identity;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/database")]
    [Authorize(Roles = "Admin")]
    public class AdminDatabaseController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminDatabaseController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminDatabaseController(
            WixiDbContext db,
            ILogger<AdminDatabaseController> logger,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            try
            {
                var stats = new
                {
                    clients = await _db.Clients.CountAsync(),
                    documents = await _db.Documents.CountAsync(),
                    applications = await _db.Applications.CountAsync(),
                    supportTickets = await _db.SupportTickets.CountAsync(),
                    users = await _db.Users.CountAsync(),
                    emailLogs = await _db.EmailLogs.CountAsync(),
                    formSubmissions = new
                    {
                        employee = await _db.EmployeeFormSubmissions.CountAsync(),
                        employer = await _db.EmployerFormSubmissions.CountAsync(),
                        contact = await _db.ContactFormSubmissions.CountAsync()
                    }
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database statistics");
                return StatusCode(500, new { success = false, message = "Error getting database statistics" });
            }
        }

        /// <summary>
        /// Delete all data for a specific client (DANGEROUS - Use with caution!)
        /// This will delete all data related to a specific client including documents, applications, support tickets, etc.
        /// </summary>
        [HttpPost("cleanup-client-data/{clientId}")]
        public async Task<ActionResult> CleanupClientData(int clientId, [FromQuery] bool deleteUser = false)
        {
            try
            {
                _logger.LogWarning("Client data cleanup started for client {ClientId} by user: {UserId}", clientId, User.Identity?.Name);

                var client = await _db.Clients
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == clientId);

                if (client == null)
                {
                    return NotFound(new { success = false, message = "Client not found" });
                }

                using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    var deletedCounts = new Dictionary<string, int>();

                    // 1. Support System
                    var supportMessages = await _db.SupportMessages
                        .Where(m => m.Ticket.ClientId == clientId)
                        .ToListAsync();
                    var supportMessagesCount = supportMessages.Count;
                    _db.SupportMessages.RemoveRange(supportMessages);
                    await _db.SaveChangesAsync();
                    deletedCounts["supportMessages"] = supportMessagesCount;
                    _logger.LogInformation("Deleted {Count} support messages for client {ClientId}", supportMessagesCount, clientId);

                    var supportTickets = await _db.SupportTickets
                        .Where(t => t.ClientId == clientId)
                        .ToListAsync();
                    var supportTicketsCount = supportTickets.Count;
                    _db.SupportTickets.RemoveRange(supportTickets);
                    await _db.SaveChangesAsync();
                    deletedCounts["supportTickets"] = supportTicketsCount;
                    _logger.LogInformation("Deleted {Count} support tickets for client {ClientId}", supportTicketsCount, clientId);

                    // 2. Notifications
                    var notifications = await _db.Notifications
                        .Where(n => n.UserId == client.UserId)
                        .ToListAsync();
                    var notificationsCount = notifications.Count;
                    _db.Notifications.RemoveRange(notifications);
                    await _db.SaveChangesAsync();
                    deletedCounts["notifications"] = notificationsCount;
                    _logger.LogInformation("Deleted {Count} notifications for client {ClientId}", notificationsCount, clientId);

                    // 3. Application System
                    var applications = await _db.Applications
                        .Where(a => a.ClientId == clientId)
                        .ToListAsync();

                    foreach (var app in applications)
                    {
                        // Delete application histories
                        var histories = await _db.ApplicationHistories
                            .Where(h => h.ApplicationId == app.Id)
                            .ToListAsync();
                        _db.ApplicationHistories.RemoveRange(histories);

                        // Delete application sub-steps
                        var subSteps = await _db.ApplicationSubSteps
                            .Where(ss => ss.Step.ApplicationId == app.Id)
                            .ToListAsync();
                        _db.ApplicationSubSteps.RemoveRange(subSteps);

                        // Delete application steps
                        var steps = await _db.ApplicationSteps
                            .Where(s => s.ApplicationId == app.Id)
                            .ToListAsync();
                        _db.ApplicationSteps.RemoveRange(steps);
                    }

                    await _db.SaveChangesAsync();
                    var applicationsCount = applications.Count;
                    var applicationIds = applications.Select(a => a.Id).ToList();
                    _db.Applications.RemoveRange(applications);
                    await _db.SaveChangesAsync();
                    deletedCounts["applications"] = applicationsCount;
                    var historiesCount = await _db.ApplicationHistories.CountAsync(h => applicationIds.Contains(h.ApplicationId));
                    deletedCounts["applicationHistories"] = historiesCount;
                    _logger.LogInformation("Deleted {Count} applications for client {ClientId}", applicationsCount, clientId);

                    // 4. Document System
                    var documentReviews = await _db.DocumentReviews
                        .Where(dr => dr.Document.ClientId == clientId)
                        .ToListAsync();
                    var documentReviewsCount = documentReviews.Count;
                    _db.DocumentReviews.RemoveRange(documentReviews);
                    await _db.SaveChangesAsync();
                    deletedCounts["documentReviews"] = documentReviewsCount;
                    _logger.LogInformation("Deleted {Count} document reviews for client {ClientId}", documentReviewsCount, clientId);

                    var documents = await _db.Documents
                        .Where(d => d.ClientId == clientId)
                        .ToListAsync();
                    var documentsCount = documents.Count;
                    _db.Documents.RemoveRange(documents);
                    await _db.SaveChangesAsync();
                    deletedCounts["documents"] = documentsCount;
                    _logger.LogInformation("Deleted {Count} documents for client {ClientId}", documentsCount, clientId);

                    // 5. Education Info
                    var educationInfos = await _db.EducationInfos
                        .Where(e => e.ClientId == clientId)
                        .ToListAsync();
                    var educationInfosCount = educationInfos.Count;
                    _db.EducationInfos.RemoveRange(educationInfos);
                    await _db.SaveChangesAsync();
                    deletedCounts["educationInfos"] = educationInfosCount;
                    _logger.LogInformation("Deleted {Count} education infos for client {ClientId}", educationInfosCount, clientId);

                    // 6. Pending Client Code (if exists)
                    var pendingCode = await _db.PendingClientCodes
                        .FirstOrDefaultAsync(p => p.ClientCode == client.ClientCode);
                    if (pendingCode != null)
                    {
                        _db.PendingClientCodes.Remove(pendingCode);
                        await _db.SaveChangesAsync();
                        deletedCounts["pendingClientCode"] = 1;
                    }

                    // 7. Client
                    _db.Clients.Remove(client);
                    await _db.SaveChangesAsync();
                    deletedCounts["client"] = 1;
                    _logger.LogInformation("Deleted client {ClientId}", clientId);

                    // 8. User (Optional)
                    if (deleteUser && client.UserId > 0)
                    {
                        var userId = client.UserId;
                        
                        // Remove all Identity relationships using SQL (to avoid EF tracking issues)
                        await _db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = {0}", userId);
                        await _db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserClaims] WHERE [UserId] = {0}", userId);
                        await _db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserLogins] WHERE [UserId] = {0}", userId);
                        await _db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUserTokens] WHERE [UserId] = {0}", userId);
                        
                        // Delete user using SQL
                        await _db.Database.ExecuteSqlRawAsync(
                            "DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = {0}", userId);
                        
                        deletedCounts["user"] = 1;
                        _logger.LogInformation("Deleted user {UserId} for client {ClientId}", userId, clientId);
                    }

                    await transaction.CommitAsync();

                    var summary = new
                    {
                        success = true,
                        message = $"Client {client.FullName} ve tüm bağlı verileri başarıyla silindi",
                        clientName = client.FullName,
                        clientCode = client.ClientCode,
                        deleted = deletedCounts
                    };

                    _logger.LogWarning("Client data cleanup completed successfully for client {ClientId} by user: {UserId}", clientId, User.Identity?.Name);
                    return Ok(summary);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during client data cleanup - transaction rolled back for client {ClientId}", clientId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up client data for client {ClientId}", clientId);
                return StatusCode(500, new { success = false, message = "Error cleaning up client data: " + ex.Message });
            }
        }

        /// <summary>
        /// Clean up test data (DANGEROUS - Use with caution!)
        /// This will delete all test data including clients, documents, applications, etc.
        /// </summary>
        [HttpPost("cleanup-test-data")]
        public async Task<ActionResult> CleanupTestData([FromQuery] bool includeUsers = false)
        {
            try
            {
                _logger.LogWarning("Test data cleanup started by user: {UserId}", User.Identity?.Name);

                using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    // 1. Support System (En alt seviye)
                    var supportMessagesCount = await _db.SupportMessages.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_SupportMessage]");
                    _logger.LogInformation("Deleted {Count} support messages", supportMessagesCount);

                    var supportTicketsCount = await _db.SupportTickets.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_SupportTicket]");
                    _logger.LogInformation("Deleted {Count} support tickets", supportTicketsCount);

                    // 2. Notifications
                    var notificationsCount = await _db.Notifications.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_Notification]");
                    _logger.LogInformation("Deleted {Count} notifications", notificationsCount);

                    // 3. Application System
                    var applicationHistoriesCount = await _db.ApplicationHistories.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_ApplicationHistory]");
                    _logger.LogInformation("Deleted {Count} application histories", applicationHistoriesCount);

                    var applicationSubStepsCount = await _db.ApplicationSubSteps.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_ApplicationSubStep]");
                    _logger.LogInformation("Deleted {Count} application sub-steps", applicationSubStepsCount);

                    var applicationStepsCount = await _db.ApplicationSteps.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_ApplicationStep]");
                    _logger.LogInformation("Deleted {Count} application steps", applicationStepsCount);

                    var applicationsCount = await _db.Applications.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_Application]");
                    _logger.LogInformation("Deleted {Count} applications", applicationsCount);

                    // 4. Document System
                    var documentReviewsCount = await _db.DocumentReviews.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_DocumentReview]");
                    _logger.LogInformation("Deleted {Count} document reviews", documentReviewsCount);

                    var documentsCount = await _db.Documents.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_Document]");
                    _logger.LogInformation("Deleted {Count} documents", documentsCount);

                    var fileStoragesCount = await _db.FileStorages.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_FileStorage]");
                    _logger.LogInformation("Deleted {Count} file storages", fileStoragesCount);

                    // 5. Client System
                    var educationInfosCount = await _db.EducationInfos.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_EducationInfo]");
                    _logger.LogInformation("Deleted {Count} education infos", educationInfosCount);

                    var pendingClientCodesCount = await _db.PendingClientCodes.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_PendingClientCode]");
                    _logger.LogInformation("Deleted {Count} pending client codes", pendingClientCodesCount);

                    var clientsCount = await _db.Clients.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[wixi_Client]");
                    _logger.LogInformation("Deleted {Count} clients", clientsCount);

                    // 6. Form Submissions
                    var employeeSubmissionsCount = await _db.EmployeeFormSubmissions.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[EmployeeFormSubmissions]");
                    _logger.LogInformation("Deleted {Count} employee form submissions", employeeSubmissionsCount);

                    var employerSubmissionsCount = await _db.EmployerFormSubmissions.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[EmployerFormSubmissions]");
                    _logger.LogInformation("Deleted {Count} employer form submissions", employerSubmissionsCount);

                    var contactSubmissionsCount = await _db.ContactFormSubmissions.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[ContactFormSubmissions]");
                    _logger.LogInformation("Deleted {Count} contact form submissions", contactSubmissionsCount);

                    // 7. Email Logs
                    var emailLogsCount = await _db.EmailLogs.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[EmailLogs]");
                    _logger.LogInformation("Deleted {Count} email logs", emailLogsCount);

                    // 8. User Preferences
                    var userPreferencesCount = await _db.UserPreferences.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[UserPreferences]");
                    _logger.LogInformation("Deleted {Count} user preferences", userPreferencesCount);

                    // 9. Application Logs
                    var applicationLogsCount = await _db.ApplicationLogs.CountAsync();
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[ApplicationLogs]");
                    _logger.LogInformation("Deleted {Count} application logs", applicationLogsCount);

                    // 10. Users (Optional - only if includeUsers=true)
                    if (includeUsers)
                    {
                        // First, remove user roles
                        await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUserRoles]");
                        _logger.LogInformation("Deleted user roles");

                        // Remove user claims
                        await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUserClaims]");
                        _logger.LogInformation("Deleted user claims");

                        // Remove user logins
                        await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUserLogins]");
                        _logger.LogInformation("Deleted user logins");

                        // Remove user tokens
                        await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUserTokens]");
                        _logger.LogInformation("Deleted user tokens");

                        // Get admin role IDs to preserve admin users
                        var adminRoleIds = await _db.Roles
                            .Where(r => r.Name == "Admin" || r.Name == "SuperAdmin")
                            .Select(r => r.Id)
                            .ToListAsync();

                        if (adminRoleIds.Any())
                        {
                            // Delete users that are NOT admins
                            var adminUserIds = await _db.UserRoles
                                .Where(ur => adminRoleIds.Contains(ur.RoleId))
                                .Select(ur => ur.UserId)
                                .Distinct()
                                .ToListAsync();

                            if (adminUserIds.Any())
                            {
                                var usersToDelete = await _db.Users
                                    .Where(u => !adminUserIds.Contains(u.Id))
                                    .ToListAsync();

                                var usersCount = usersToDelete.Count;
                                foreach (var user in usersToDelete)
                                {
                                    await _userManager.DeleteAsync(user);
                                }
                                _logger.LogInformation("Deleted {Count} non-admin users", usersCount);
                            }
                            else
                            {
                                // If no admin users found, delete all users
                                var allUsersCount = await _db.Users.CountAsync();
                                await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUsers]");
                                _logger.LogInformation("Deleted {Count} users (no admin users found)", allUsersCount);
                            }
                        }
                        else
                        {
                            // If no admin roles found, delete all users
                            var allUsersCount = await _db.Users.CountAsync();
                            await _db.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[AspNetUsers]");
                            _logger.LogInformation("Deleted {Count} users (no admin roles found)", allUsersCount);
                        }
                    }

                    await transaction.CommitAsync();

                    var summary = new
                    {
                        success = true,
                        message = "Test data cleanup completed successfully",
                        deleted = new
                        {
                            supportMessages = supportMessagesCount,
                            supportTickets = supportTicketsCount,
                            notifications = notificationsCount,
                            applicationHistories = applicationHistoriesCount,
                            applicationSubSteps = applicationSubStepsCount,
                            applicationSteps = applicationStepsCount,
                            applications = applicationsCount,
                            documentReviews = documentReviewsCount,
                            documents = documentsCount,
                            fileStorages = fileStoragesCount,
                            educationInfos = educationInfosCount,
                            pendingClientCodes = pendingClientCodesCount,
                            clients = clientsCount,
                            employeeSubmissions = employeeSubmissionsCount,
                            employerSubmissions = employerSubmissionsCount,
                            contactSubmissions = contactSubmissionsCount,
                            emailLogs = emailLogsCount,
                            userPreferences = userPreferencesCount,
                            applicationLogs = applicationLogsCount
                        }
                    };

                    _logger.LogWarning("Test data cleanup completed successfully by user: {UserId}", User.Identity?.Name);
                    return Ok(summary);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during test data cleanup - transaction rolled back");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up test data");
                return StatusCode(500, new { success = false, message = "Error cleaning up test data: " + ex.Message });
            }
        }

        /// <summary>
        /// Clean up all data for a specific client (DANGEROUS - Use with caution!)
        /// This will delete the client and all related data (documents, applications, support tickets, etc.)
        /// </summary>
        [HttpDelete("cleanup-client/{clientId}")]
        public async Task<ActionResult> CleanupClientData(int clientId)
        {
            try
            {
                _logger.LogWarning("Client data cleanup started for client {ClientId} by user: {UserId}", clientId, User.Identity?.Name);

                var client = await _db.Clients
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == clientId);

                if (client == null)
                {
                    return NotFound(new { success = false, message = "Client not found" });
                }

                using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    var deletedCounts = new Dictionary<string, int>();

                    // 1. Support System
                    var supportMessages = await _db.SupportMessages
                        .Where(m => m.Ticket.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["supportMessages"] = supportMessages.Count;
                    _db.SupportMessages.RemoveRange(supportMessages);

                    var supportTickets = await _db.SupportTickets
                        .Where(t => t.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["supportTickets"] = supportTickets.Count;
                    _db.SupportTickets.RemoveRange(supportTickets);

                    // 2. Notifications
                    var notifications = await _db.Notifications
                        .Where(n => n.UserId == client.UserId)
                        .ToListAsync();
                    deletedCounts["notifications"] = notifications.Count;
                    _db.Notifications.RemoveRange(notifications);

                    // 3. Application System
                    var applicationHistories = await _db.ApplicationHistories
                        .Where(h => h.Application.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["applicationHistories"] = applicationHistories.Count;
                    _db.ApplicationHistories.RemoveRange(applicationHistories);

                    var applicationSubSteps = await _db.ApplicationSubSteps
                        .Where(ss => ss.Step.Application.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["applicationSubSteps"] = applicationSubSteps.Count;
                    _db.ApplicationSubSteps.RemoveRange(applicationSubSteps);

                    var applicationSteps = await _db.ApplicationSteps
                        .Where(s => s.Application.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["applicationSteps"] = applicationSteps.Count;
                    _db.ApplicationSteps.RemoveRange(applicationSteps);

                    var applications = await _db.Applications
                        .Where(a => a.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["applications"] = applications.Count;
                    _db.Applications.RemoveRange(applications);

                    // 4. Document System
                    var documentReviews = await _db.DocumentReviews
                        .Where(r => r.Document.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["documentReviews"] = documentReviews.Count;
                    _db.DocumentReviews.RemoveRange(documentReviews);

                    var documents = await _db.Documents
                        .Where(d => d.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["documents"] = documents.Count;
                    _db.Documents.RemoveRange(documents);

                    // 5. Client System
                    var educationInfos = await _db.EducationInfos
                        .Where(e => e.ClientId == clientId)
                        .ToListAsync();
                    deletedCounts["educationInfos"] = educationInfos.Count;
                    _db.EducationInfos.RemoveRange(educationInfos);

                    // 6. Delete client
                    _db.Clients.Remove(client);
                    deletedCounts["clients"] = 1;

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogWarning("Client data cleanup completed for client {ClientId} by user: {UserId}", clientId, User.Identity?.Name);

                    return Ok(new
                    {
                        success = true,
                        message = $"Client '{client.FullName}' and all related data deleted successfully",
                        deleted = deletedCounts,
                        clientName = client.FullName,
                        clientCode = client.ClientCode
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during client data cleanup - transaction rolled back");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up client data for client {ClientId}", clientId);
                return StatusCode(500, new { success = false, message = "Error cleaning up client data: " + ex.Message });
            }
        }

        /// <summary>
        /// Clean up all data for a specific user (DANGEROUS - Use with caution!)
        /// This will delete the user, their client profile, and all related data
        /// </summary>
        [HttpDelete("cleanup-user/{userId}")]
        public async Task<ActionResult> CleanupUserData(int userId)
        {
            try
            {
                _logger.LogWarning("User data cleanup started for user {UserId} by admin: {AdminUserId}", userId, User.Identity?.Name);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Check if user is admin
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "SuperAdmin");
                if (isAdmin)
                {
                    return BadRequest(new { success = false, message = "Cannot delete admin users. Please remove admin role first." });
                }

                // Get client if exists
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == userId);

                using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    var deletedCounts = new Dictionary<string, int>();

                    // If client exists, delete all client-related data first
                    if (client != null)
                    {
                        // 1. Support System
                        var supportMessages = await _db.SupportMessages
                            .Where(m => m.Ticket.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["supportMessages"] = supportMessages.Count;
                        _db.SupportMessages.RemoveRange(supportMessages);

                        var supportTickets = await _db.SupportTickets
                            .Where(t => t.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["supportTickets"] = supportTickets.Count;
                        _db.SupportTickets.RemoveRange(supportTickets);

                        // 2. Application System
                        var applicationHistories = await _db.ApplicationHistories
                            .Where(h => h.Application.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["applicationHistories"] = applicationHistories.Count;
                        _db.ApplicationHistories.RemoveRange(applicationHistories);

                        var applicationSubSteps = await _db.ApplicationSubSteps
                            .Where(ss => ss.Step.Application.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["applicationSubSteps"] = applicationSubSteps.Count;
                        _db.ApplicationSubSteps.RemoveRange(applicationSubSteps);

                        var applicationSteps = await _db.ApplicationSteps
                            .Where(s => s.Application.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["applicationSteps"] = applicationSteps.Count;
                        _db.ApplicationSteps.RemoveRange(applicationSteps);

                        var applications = await _db.Applications
                            .Where(a => a.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["applications"] = applications.Count;
                        _db.Applications.RemoveRange(applications);

                        // 3. Document System
                        var documentReviews = await _db.DocumentReviews
                            .Where(r => r.Document.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["documentReviews"] = documentReviews.Count;
                        _db.DocumentReviews.RemoveRange(documentReviews);

                        var documents = await _db.Documents
                            .Where(d => d.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["documents"] = documents.Count;
                        _db.Documents.RemoveRange(documents);

                        // 4. Client System
                        var educationInfos = await _db.EducationInfos
                            .Where(e => e.ClientId == client.Id)
                            .ToListAsync();
                        deletedCounts["educationInfos"] = educationInfos.Count;
                        _db.EducationInfos.RemoveRange(educationInfos);

                        // Delete client
                        _db.Clients.Remove(client);
                        deletedCounts["clients"] = 1;
                    }

                    // 5. User-related data
                    var notifications = await _db.Notifications
                        .Where(n => n.UserId == userId)
                        .ToListAsync();
                    deletedCounts["notifications"] = notifications.Count;
                    _db.Notifications.RemoveRange(notifications);

                    var userPreferences = await _db.UserPreferences
                        .Where(up => up.UserId == userId.ToString())
                        .ToListAsync();
                    deletedCounts["userPreferences"] = userPreferences.Count;
                    _db.UserPreferences.RemoveRange(userPreferences);

                    // 6. Remove user roles (using raw SQL to avoid DbSet issues)
                    var userRolesCount = await _db.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = {0}", userId);
                    deletedCounts["userRoles"] = (int)userRolesCount;

                    // 7. Remove user claims
                    var userClaimsCount = await _db.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [dbo].[AspNetUserClaims] WHERE [UserId] = {0}", userId);
                    deletedCounts["userClaims"] = (int)userClaimsCount;

                    // 8. Remove user logins
                    var userLoginsCount = await _db.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [dbo].[AspNetUserLogins] WHERE [UserId] = {0}", userId);
                    deletedCounts["userLogins"] = (int)userLoginsCount;

                    // 9. Remove user tokens
                    var userTokensCount = await _db.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [dbo].[AspNetUserTokens] WHERE [UserId] = {0}", userId);
                    deletedCounts["userTokens"] = (int)userTokensCount;

                    // 10. Delete user using SQL (to avoid EF relationship issues)
                    var usersDeleted = await _db.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = {0}", userId);
                    deletedCounts["users"] = usersDeleted > 0 ? 1 : 0;
                    await transaction.CommitAsync();

                    _logger.LogWarning("User data cleanup completed for user {UserId} by admin: {AdminUserId}", userId, User.Identity?.Name);

                    return Ok(new
                    {
                        success = true,
                        message = $"User '{user.Email}' and all related data deleted successfully",
                        deleted = deletedCounts,
                        userName = user.Email,
                        hadClient = client != null
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during user data cleanup - transaction rolled back");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up user data for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Error cleaning up user data: " + ex.Message });
            }
        }
    }
}

