using DoAn4.DTOs;

namespace DoAn4.Services.ConversationService
{
    public interface IConversationService
    {
        Task<List<ConversationDto>> GetAllConversations(string token );
        Task<bool> DeleteConversation(Guid conversationId);
    }
}
