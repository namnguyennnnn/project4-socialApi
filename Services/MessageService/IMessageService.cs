using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Services.MessageService
{
    public interface IMessageService
    {
        Task<bool> SendMessage(string token , Guid recipientId, string content);
        Task<List<Message>> GetMessages(Guid conversationId);
    }
}
