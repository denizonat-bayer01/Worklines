using wixi.Entities.DTOs;

namespace wixi.Business.Abstract
{
    public interface ISupportService
    {
        // Ticket operations
        Task<SupportTicketResponseDto> CreateTicketAsync(SupportTicketCreateDto createDto);
        Task<SupportTicketResponseDto> GetTicketByIdAsync(long ticketId);
        Task<List<SupportTicketResponseDto>> GetClientTicketsAsync(int clientId);
        Task<List<SupportTicketResponseDto>> GetAllTicketsAsync();
        Task<SupportTicketResponseDto> UpdateTicketAsync(long ticketId, SupportTicketUpdateDto updateDto);
        Task<bool> DeleteTicketAsync(long ticketId);
        Task<SupportTicketResponseDto> AssignTicketAsync(long ticketId, TicketAssignDto assignDto);
        Task<SupportTicketResponseDto> UpdateTicketStatusAsync(long ticketId, TicketStatusUpdateDto statusDto);
        
        // Message operations
        Task<SupportMessageDto> AddMessageAsync(SupportMessageCreateDto createDto);
        Task<List<SupportMessageDto>> GetTicketMessagesAsync(long ticketId);
        Task<bool> MarkMessageAsReadAsync(long messageId);
        
        // FAQ operations
        Task<List<FAQDto>> GetAllFAQsAsync();
        Task<List<FAQDto>> GetFAQsByCategoryAsync(string category);
        Task<FAQDto> GetFAQByIdAsync(int faqId);
        Task<FAQDto> CreateFAQAsync(FAQCreateDto createDto);
        Task<FAQDto> UpdateFAQAsync(int faqId, FAQUpdateDto updateDto);
        Task<bool> DeleteFAQAsync(int faqId);
    }
}

