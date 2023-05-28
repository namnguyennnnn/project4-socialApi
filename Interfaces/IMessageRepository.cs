using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Interfaces
{
    public interface IMessageRepository
    {

        Task<Message> SendMessage(Message message);
        Task<List<Message>> GetMessages(Guid conversationId);
        Task<Message> GetLastMessage(Guid conversationId);
        Task<List<Message>> GetLastMessages(List<Guid> conversationIds);
        Task<bool> DeleteMessagesByConversationId(Guid conversationId);
    }
}
