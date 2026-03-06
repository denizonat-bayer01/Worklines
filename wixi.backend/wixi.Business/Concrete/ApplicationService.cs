using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.DTOs;

namespace wixi.Business.Concrete
{
    public class ApplicationService : IApplicationService
    {
        private readonly WixiDbContext _context;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            WixiDbContext context,
            ILogger<ApplicationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApplicationResponseDto> CreateApplicationAsync(ApplicationCreateDto createDto)
        {
            // Validate client
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == createDto.ClientId);
            if (!clientExists)
            {
                throw new Exception("Client not found");
            }

            // Get template with steps
            var template = await _context.ApplicationTemplates
                .Include(t => t.StepTemplates)
                    .ThenInclude(st => st.SubStepTemplates)
                .FirstOrDefaultAsync(t => t.Id == createDto.TemplateId && t.IsActive);

            if (template == null)
            {
                throw new Exception("Application template not found or inactive");
            }

            // Create application
            var application = new Entities.Concrete.Application.Application
            {
                ClientId = createDto.ClientId,
                ApplicationTemplateId = createDto.TemplateId,
                Status = ApplicationStatus.InProgress, // Start application as InProgress
                StartDate = DateTime.UtcNow,
                ExpectedCompletionDate = DateTime.UtcNow.AddDays(template.EstimatedDurationDays ?? 30),
                Notes = createDto.Notes,
                ApplicationNumber = $"APP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // Create steps from template
            bool isFirstStep = true;
            foreach (var stepTemplate in template.StepTemplates.OrderBy(st => st.StepOrder))
            {
                var step = new Entities.Concrete.Application.ApplicationStep
                {
                    ApplicationId = application.Id,
                    StepTemplateId = stepTemplate.Id,
                    Title = stepTemplate.Title,
                    StepOrder = stepTemplate.StepOrder,
                    Status = isFirstStep ? StepStatus.InProgress : StepStatus.NotStarted
                };

                if (isFirstStep)
                {
                    step.StartDate = DateTime.UtcNow;
                }

                _context.ApplicationSteps.Add(step);
                await _context.SaveChangesAsync();

                // Create sub-steps from template
                bool isFirstSubStep = true;
                foreach (var subStepTemplate in stepTemplate.SubStepTemplates.OrderBy(sst => sst.SubStepOrder))
                {
                    var isFirstSubStepOfFirstStep = isFirstStep && isFirstSubStep;
                    var subStepStatus = isFirstSubStepOfFirstStep ? StepStatus.InProgress : StepStatus.NotStarted;
                    
                    var subStep = new Entities.Concrete.Application.ApplicationSubStep
                    {
                        ApplicationStepId = step.Id,
                        SubStepTemplateId = subStepTemplate.Id,
                        Name = subStepTemplate.Name,
                        SubStepOrder = subStepTemplate.SubStepOrder,
                        // If this is the first step's first sub-step, start it immediately
                        Status = subStepStatus
                    };

                    // Log the first sub-step creation
                    if (isFirstSubStepOfFirstStep)
                    {
                        _logger.LogInformation(
                            "Creating first sub-step '{SubStepName}' (Order: {Order}) with Status: {Status} for first step '{StepTitle}' (Order: {StepOrder}) in Application {ApplicationId}",
                            subStepTemplate.Name, subStepTemplate.SubStepOrder, subStepStatus, stepTemplate.Title, stepTemplate.StepOrder, application.Id);
                    }

                    _context.ApplicationSubSteps.Add(subStep);
                    isFirstSubStep = false;
                }

                isFirstStep = false;
            }

            await _context.SaveChangesAsync();

            // Add history entry - Get client to access UserId
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == createDto.ClientId);
            
            if (client == null)
            {
                _logger.LogWarning("Client not found when creating application history - ClientId: {ClientId}", createDto.ClientId);
            }
            else
            {
                var history = new Entities.Concrete.Application.ApplicationHistory
                {
                    ApplicationId = application.Id,
                    Action = "Created",
                    Description = $"Application created using template: {template.Name}",
                    UserId = client.UserId, // Use UserId from Client, not ClientId
                    UserType = "Client",
                    CreatedAt = DateTime.UtcNow
                };
                
                // Only add history if UserId exists (user must be created first)
                if (client.UserId > 0)
                {
                    _context.ApplicationHistories.Add(history);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Application history created - ApplicationId: {ApplicationId}, UserId: {UserId}", 
                        application.Id, client.UserId);
                }
                else
                {
                    _logger.LogWarning("Skipping application history creation - Client UserId is 0 - ClientId: {ClientId}", 
                        createDto.ClientId);
                }
            }

            _logger.LogInformation("Application created: {ApplicationId} for client {ClientId}",
                application.Id, createDto.ClientId);

            return await GetApplicationByIdAsync(application.Id);
        }

        public async Task<ApplicationResponseDto> GetApplicationByIdAsync(long applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Include(a => a.Template)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.SubSteps)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            return MapToApplicationDto(application);
        }

        public async Task<List<ApplicationResponseDto>> GetClientApplicationsAsync(int clientId)
        {
            var applications = await _context.Applications
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Include(a => a.Template)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.SubSteps)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();

            return applications.Select(a => MapToApplicationDto(a)).ToList();
        }

        public async Task<List<ApplicationResponseDto>> GetAllApplicationsAsync()
        {
            var applications = await _context.Applications
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Include(a => a.Template)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.SubSteps)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();

            return applications.Select(a => MapToApplicationDto(a)).ToList();
        }

        public async Task<ApplicationResponseDto> UpdateApplicationAsync(long applicationId, ApplicationUpdateDto updateDto)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            var oldNotes = application.Notes;
            application.Notes = updateDto.Notes;
            application.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Add history
            var history = new Entities.Concrete.Application.ApplicationHistory
            {
                ApplicationId = applicationId,
                Action = "Updated",
                Description = "Application notes updated",
                OldValue = oldNotes,
                NewValue = updateDto.Notes,
                UserId = 1, // TODO: Get from authenticated user
                UserType = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            _context.ApplicationHistories.Add(history);
            await _context.SaveChangesAsync();

            return await GetApplicationByIdAsync(applicationId);
        }

        public async Task<bool> DeleteApplicationAsync(long applicationId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Application deleted: {ApplicationId}", applicationId);
            return true;
        }

        public async Task<ApplicationStepResponseDto> UpdateStepAsync(long stepId, StepUpdateDto updateDto, int userId)
        {
            var step = await _context.ApplicationSteps
                .Include(s => s.Application)
                .Include(s => s.SubSteps)
                .FirstOrDefaultAsync(s => s.Id == stepId);

            if (step == null)
            {
                throw new Exception("Step not found");
            }

            var oldStatus = step.Status.ToString();

            // Parse and update status
            if (Enum.TryParse<StepStatus>(updateDto.Status, true, out var newStatus))
            {
                step.Status = newStatus;

                if (newStatus == StepStatus.InProgress && step.StartDate == null)
                {
                    step.StartDate = DateTime.UtcNow;
                }
                else if (newStatus == StepStatus.Completed)
                {
                    step.CompletionDate = DateTime.UtcNow;

                    // Update application progress
                    await UpdateApplicationProgress(step.ApplicationId);
                }
            }

            step.Notes = updateDto.Notes;
            step.AssignedTo = updateDto.AssignedToUserId?.ToString();
            step.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Add history
            var history = new Entities.Concrete.Application.ApplicationHistory
            {
                ApplicationId = step.ApplicationId,
                Action = "StepUpdated",
                Description = $"Step '{step.Title}' status updated",
                OldValue = oldStatus,
                NewValue = newStatus.ToString(),
                UserId = userId,
                UserType = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            _context.ApplicationHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Step {StepId} updated to status {Status}", stepId, newStatus);

            return MapToStepDto(step);
        }

        public async Task<ApplicationSubStepResponseDto> UpdateSubStepAsync(long subStepId, SubStepUpdateDto updateDto, int userId)
        {
            var subStep = await _context.ApplicationSubSteps
                .Include(ss => ss.Step)
                    .ThenInclude(s => s.Application)
                .FirstOrDefaultAsync(ss => ss.Id == subStepId);

            if (subStep == null)
            {
                throw new Exception("Sub-step not found");
            }

            var newStatus = updateDto.IsCompleted ? StepStatus.Completed : StepStatus.NotStarted;
            subStep.Status = newStatus;
            subStep.Notes = updateDto.Notes;

            if (updateDto.IsCompleted && subStep.CompletionDate == null)
            {
                subStep.CompletionDate = DateTime.UtcNow;
            }
            else if (!updateDto.IsCompleted)
            {
                subStep.CompletionDate = null;
            }

            subStep.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Add history
            var history = new Entities.Concrete.Application.ApplicationHistory
            {
                ApplicationId = subStep.Step.ApplicationId,
                Action = "SubStepUpdated",
                Description = $"Sub-step '{subStep.Name}' marked as {(updateDto.IsCompleted ? "completed" : "incomplete")}",
                UserId = userId,
                UserType = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            _context.ApplicationHistories.Add(history);
            await _context.SaveChangesAsync();

            return MapToSubStepDto(subStep);
        }

        public async Task<List<ApplicationHistoryDto>> GetApplicationHistoryAsync(long applicationId)
        {
            var history = await _context.ApplicationHistories
                .Include(h => h.User)
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return history.Select(h => new ApplicationHistoryDto
            {
                Id = h.Id,
                ApplicationId = h.ApplicationId,
                Action = h.Action,
                Description = h.Description,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedByUserId = h.UserId ?? 0,
                ChangedByUserName = h.User != null ? $"{h.User.FirstName} {h.User.LastName}" : "System",
                ChangedAt = h.CreatedAt
            }).ToList();
        }

        public async Task<List<ApplicationTemplateDto>> GetApplicationTemplatesAsync()
        {
            var templates = await _context.ApplicationTemplates
                .Include(t => t.StepTemplates)
                    .ThenInclude(st => st.SubStepTemplates)
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();

            return templates.Select(t => MapToTemplateDto(t)).ToList();
        }

        public async Task<ApplicationTemplateDto> GetTemplateByIdAsync(int templateId)
        {
            var template = await _context.ApplicationTemplates
                .Include(t => t.StepTemplates)
                    .ThenInclude(st => st.SubStepTemplates)
                .FirstOrDefaultAsync(t => t.Id == templateId && t.IsActive);

            if (template == null)
            {
                throw new Exception("Template not found");
            }

            return MapToTemplateDto(template);
        }

        // Helper methods
        private async Task UpdateApplicationProgress(long applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Steps)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) return;

            var totalSteps = application.Steps.Count;
            var completedSteps = application.Steps.Count(s => s.Status == StepStatus.Completed);

            // Calculate progress percentage
            application.ProgressPercentage = totalSteps > 0 ? (int)((double)completedSteps / totalSteps * 100) : 0;

            // Check if all steps are completed
            if (completedSteps == totalSteps)
            {
                application.Status = ApplicationStatus.Completed;
                application.CompletionDate = DateTime.UtcNow;
            }
            else if (completedSteps > 0)
            {
                application.Status = ApplicationStatus.InProgress;
            }

            application.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        private ApplicationResponseDto MapToApplicationDto(Entities.Concrete.Application.Application application)
        {
            var totalSteps = application.Steps.Count;
            var completedSteps = application.Steps.Count(s => s.Status == StepStatus.Completed);
            var currentStep = application.Steps
                .Where(s => s.Status == StepStatus.InProgress)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();

            return new ApplicationResponseDto
            {
                Id = application.Id,
                ClientId = application.ClientId,
                ClientName = $"{application.Client.FirstName} {application.Client.LastName}",
                TemplateId = application.ApplicationTemplateId,
                TemplateName = application.Template.Name,
                Status = application.Status.ToString(),
                CurrentStepOrder = currentStep?.StepOrder ?? 1,
                CurrentStepName = currentStep?.Title,
                TotalSteps = totalSteps,
                CompletedSteps = completedSteps,
                ProgressPercentage = totalSteps > 0 ? (double)completedSteps / totalSteps * 100 : 0,
                StartDate = application.StartDate,
                CompletionDate = application.CompletionDate,
                EstimatedCompletionDate = application.ExpectedCompletionDate,
                Notes = application.Notes,
                Steps = application.Steps.OrderBy(s => s.StepOrder).Select(s => MapToStepDto(s)).ToList()
            };
        }

        private ApplicationStepResponseDto MapToStepDto(Entities.Concrete.Application.ApplicationStep step)
        {
            return new ApplicationStepResponseDto
            {
                Id = step.Id,
                ApplicationId = step.ApplicationId,
                Name = step.Title,
                Description = step.Notes,
                StepOrder = step.StepOrder,
                Status = step.Status.ToString(),
                StartedAt = step.StartDate,
                CompletedAt = step.CompletionDate,
                Notes = step.Notes,
                AssignedToUserId = !string.IsNullOrEmpty(step.AssignedTo) && int.TryParse(step.AssignedTo, out var uid) ? uid : null,
                AssignedToUserName = step.AssignedTo,
                SubSteps = step.SubSteps?.OrderBy(ss => ss.SubStepOrder).Select(ss => MapToSubStepDto(ss)).ToList() ?? new()
            };
        }

        private ApplicationSubStepResponseDto MapToSubStepDto(Entities.Concrete.Application.ApplicationSubStep subStep)
        {
            // Ensure status is properly converted to string (InProgress, NotStarted, Completed, etc.)
            var statusString = subStep.Status.ToString();
            
            return new ApplicationSubStepResponseDto
            {
                Id = subStep.Id,
                StepId = subStep.ApplicationStepId,
                Name = subStep.Name,
                Description = subStep.InfoMessage,
                SubStepOrder = subStep.SubStepOrder,
                IsCompleted = subStep.Status == StepStatus.Completed,
                Status = statusString, // InProgress, NotStarted, Completed, etc.
                CompletedAt = subStep.CompletionDate,
                Notes = subStep.Notes
            };
        }

        private ApplicationTemplateDto MapToTemplateDto(Entities.Concrete.Application.ApplicationTemplate template)
        {
            return new ApplicationTemplateDto
            {
                Id = template.Id,
                Code = $"TMPL-{template.Id}",
                Name = template.Name,
                NameEn = template.NameEn ?? template.Name,
                Description = template.Description,
                EstimatedDurationDays = template.EstimatedDurationDays ?? 30,
                IsActive = template.IsActive,
                Steps = template.StepTemplates.OrderBy(st => st.StepOrder).Select(st => new ApplicationStepTemplateDto
                {
                    Id = st.Id,
                    Name = st.Title,
                    NameEn = st.TitleEn ?? st.Title,
                    Description = st.Description,
                    StepOrder = st.StepOrder,
                    EstimatedDurationDays = st.EstimatedDurationDays ?? 7,
                    IsRequired = st.IsRequired,
                    SubSteps = st.SubStepTemplates.OrderBy(sst => sst.SubStepOrder).Select(sst => new ApplicationSubStepTemplateDto
                    {
                        Id = sst.Id,
                        Name = sst.Name,
                        NameEn = sst.NameEn ?? sst.Name,
                        Description = sst.Description,
                        SubStepOrder = sst.SubStepOrder,
                        IsRequired = sst.IsRequired
                    }).ToList()
                }).ToList()
            };
        }
    }
}
