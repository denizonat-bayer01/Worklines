using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.DTOs;

namespace wixi.Business.Concrete
{
    public class SupportService : ISupportService
    {
        private readonly WixiDbContext _context;
        private readonly ILogger<SupportService> _logger;

        public SupportService(WixiDbContext context, ILogger<SupportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SupportTicketResponseDto> CreateTicketAsync(SupportTicketCreateDto createDto)
        {
            if (!Enum.TryParse<TicketPriority>(createDto.Priority, true, out var priority))
                priority = TicketPriority.Normal;
            if (!Enum.TryParse<TicketCategory>(createDto.Category, true, out var category))
                category = TicketCategory.General;

            // Get client to find UserId
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == createDto.ClientId && c.DeletedAt == null);
            
            if (client == null)
                throw new Exception("Client not found");

            var ticket = new SupportTicket
            {
                ClientId = createDto.ClientId,
                Subject = createDto.Subject,
                TicketNumber = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}",
                Status = TicketStatus.Open,
                Priority = priority,
                Category = category,
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Add initial message - use client's UserId, not ClientId
            var initialMessage = new SupportMessage
            {
                TicketId = ticket.Id,
                SenderId = client.UserId, // Use UserId from Client, not ClientId
                Message = createDto.Description,
                IsFromClient = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.SupportMessages.Add(initialMessage);
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticket.Id);
        }

        public async Task<SupportTicketResponseDto> GetTicketByIdAsync(long ticketId)
        {
            var ticket = await _context.SupportTickets
                .Include(t => t.Client).ThenInclude(c => c.User)
                .Include(t => t.AssignedTo)
                .Include(t => t.Messages).ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) throw new Exception("Ticket not found");
            return MapToDto(ticket);
        }

        public async Task<List<SupportTicketResponseDto>> GetClientTicketsAsync(int clientId)
        {
            var tickets = await _context.SupportTickets
                .Include(t => t.Client).ThenInclude(c => c.User)
                .Include(t => t.AssignedTo)
                .Include(t => t.Messages).ThenInclude(m => m.Sender)
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tickets.Select(t => MapToDto(t)).ToList();
        }

        public async Task<List<SupportTicketResponseDto>> GetAllTicketsAsync()
        {
            var tickets = await _context.SupportTickets
                .Include(t => t.Client).ThenInclude(c => c.User)
                .Include(t => t.AssignedTo)
                .Include(t => t.Messages).ThenInclude(m => m.Sender)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tickets.Select(t => MapToDto(t)).ToList();
        }

        public async Task<SupportTicketResponseDto> UpdateTicketAsync(long ticketId, SupportTicketUpdateDto updateDto)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found");

            ticket.Subject = updateDto.Subject;
            if (Enum.TryParse<TicketPriority>(updateDto.Priority, true, out var priority))
                ticket.Priority = priority;
            if (Enum.TryParse<TicketStatus>(updateDto.Status, true, out var status))
                ticket.Status = status;

            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId);
        }

        public async Task<bool> DeleteTicketAsync(long ticketId)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found");

            _context.SupportTickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SupportTicketResponseDto> AssignTicketAsync(long ticketId, TicketAssignDto assignDto)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found");

            ticket.AssignedToId = assignDto.AssignedToUserId;
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId);
        }

        public async Task<SupportTicketResponseDto> UpdateTicketStatusAsync(long ticketId, TicketStatusUpdateDto statusDto)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found");

            if (Enum.TryParse<TicketStatus>(statusDto.Status, true, out var status))
            {
                ticket.Status = status;
                if (status == TicketStatus.Resolved) ticket.ResolvedAt = DateTime.UtcNow;
                if (status == TicketStatus.Closed) ticket.ClosedAt = DateTime.UtcNow;
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId);
        }

        public async Task<SupportMessageDto> AddMessageAsync(SupportMessageCreateDto createDto)
        {
            var message = new SupportMessage
            {
                TicketId = createDto.TicketId,
                SenderId = createDto.SenderId,
                Message = createDto.MessageText,
                IsFromClient = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportMessages.Add(message);

            var ticket = await _context.SupportTickets.FindAsync(createDto.TicketId);
            if (ticket != null)
            {
                ticket.UpdatedAt = DateTime.UtcNow;
                if (ticket.FirstResponseAt == null && !message.IsFromClient)
                    ticket.FirstResponseAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return await GetMessageDtoAsync(message.Id);
        }

        public async Task<List<SupportMessageDto>> GetTicketMessagesAsync(long ticketId)
        {
            var messages = await _context.SupportMessages
                .Include(m => m.Sender)
                .Where(m => m.TicketId == ticketId && m.DeletedAt == null)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return messages.Select(m => MapToMessageDto(m)).ToList();
        }

        public async Task<bool> MarkMessageAsReadAsync(long messageId)
        {
            var message = await _context.SupportMessages.FindAsync(messageId);
            if (message == null) return false;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FAQDto>> GetAllFAQsAsync()
        {
            var faqs = await _context.FAQs
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();

            return faqs.Select(f => MapToFAQDto(f)).ToList();
        }

        public async Task<List<FAQDto>> GetFAQsByCategoryAsync(string category)
        {
            if (!Enum.TryParse<FAQCategory>(category, true, out var faqCategory))
                throw new Exception($"Invalid FAQ category: {category}");

            var faqs = await _context.FAQs
                .Where(f => f.IsActive && f.Category == faqCategory)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();

            return faqs.Select(f => MapToFAQDto(f)).ToList();
        }

        public async Task<FAQDto> GetFAQByIdAsync(int faqId)
        {
            var faq = await _context.FAQs.FindAsync(faqId);
            if (faq == null) throw new Exception("FAQ not found");
            return MapToFAQDto(faq);
        }

        public async Task<FAQDto> CreateFAQAsync(FAQCreateDto createDto)
        {
            if (!Enum.TryParse<FAQCategory>(createDto.Category, true, out var faqCategory))
                throw new Exception($"Invalid FAQ category: {createDto.Category}");

            var faq = new FAQ
            {
                Category = faqCategory,
                Question = createDto.Question,
                Answer = createDto.Answer,
                QuestionEn = createDto.QuestionEn,
                AnswerEn = createDto.AnswerEn,
                DisplayOrder = createDto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FAQs.Add(faq);
            await _context.SaveChangesAsync();
            return MapToFAQDto(faq);
        }

        public async Task<FAQDto> UpdateFAQAsync(int faqId, FAQUpdateDto updateDto)
        {
            var faq = await _context.FAQs.FindAsync(faqId);
            if (faq == null) throw new Exception("FAQ not found");

            if (!Enum.TryParse<FAQCategory>(updateDto.Category, true, out var faqCategory))
                throw new Exception($"Invalid FAQ category: {updateDto.Category}");

            faq.Category = faqCategory;
            faq.Question = updateDto.Question;
            faq.Answer = updateDto.Answer;
            faq.QuestionEn = updateDto.QuestionEn;
            faq.AnswerEn = updateDto.AnswerEn;
            faq.DisplayOrder = updateDto.DisplayOrder;
            faq.IsActive = updateDto.IsActive;
            faq.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToFAQDto(faq);
        }

        public async Task<bool> DeleteFAQAsync(int faqId)
        {
            var faq = await _context.FAQs.FindAsync(faqId);
            if (faq == null) throw new Exception("FAQ not found");

            _context.FAQs.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper methods
        private async Task<SupportMessageDto> GetMessageDtoAsync(long messageId)
        {
            var message = await _context.SupportMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null) throw new Exception("Message not found");
            return MapToMessageDto(message);
        }

        private SupportTicketResponseDto MapToDto(SupportTicket ticket)
        {
            return new SupportTicketResponseDto
            {
                Id = ticket.Id,
                ClientId = ticket.ClientId,
                ClientName = ticket.Client != null 
                    ? $"{ticket.Client.FirstName ?? ""} {ticket.Client.LastName ?? ""}".Trim()
                    : "Unknown Client",
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                Description = ticket.Messages?.FirstOrDefault()?.Message ?? "",
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                Category = ticket.Category.ToString(),
                AssignedToUserId = ticket.AssignedToId,
                AssignedToUserName = ticket.AssignedTo != null ? $"{ticket.AssignedTo.FirstName} {ticket.AssignedTo.LastName}" : null,
                CreatedAt = ticket.CreatedAt,
                ResolvedAt = ticket.ResolvedAt,
                LastMessageAt = ticket.Messages != null && ticket.Messages.Any() ? ticket.Messages.Max(m => m.CreatedAt) : ticket.CreatedAt,
                MessageCount = ticket.Messages?.Count ?? 0,
                Messages = ticket.Messages?.OrderBy(m => m.CreatedAt).Select(m => MapToMessageDto(m)).ToList() ?? new List<SupportMessageDto>()
            };
        }

        private SupportMessageDto MapToMessageDto(SupportMessage message)
        {
            return new SupportMessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                SenderId = message.SenderId,
                SenderName = message.Sender != null 
                    ? $"{message.Sender.FirstName ?? ""} {message.Sender.LastName ?? ""}".Trim()
                    : "Unknown Sender",
                SenderRole = message.IsFromClient ? "Client" : "Support",
                MessageText = message.Message,
                SentAt = message.CreatedAt,
                IsRead = message.IsRead
            };
        }

        private FAQDto MapToFAQDto(FAQ faq)
        {
            return new FAQDto
            {
                Id = faq.Id,
                Category = faq.Category.ToString(),
                Question = faq.Question,
                Answer = faq.Answer,
                QuestionEn = faq.QuestionEn,
                AnswerEn = faq.AnswerEn,
                DisplayOrder = faq.DisplayOrder,
                IsActive = faq.IsActive
            };
        }
    }
}

