using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<Message> SendMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetMessages(Guid conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationsId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message> GetLastMessage(Guid conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationsId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Message>> GetLastMessages(List<Guid> conversationIds)
        {
            var lastMessages = await _context.Messages
                .Where(m => conversationIds.Contains(m.ConversationsId))
                .GroupBy(m => m.ConversationsId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
                .ToListAsync();

            return lastMessages;
        }

        public async Task<bool> DeleteMessagesByConversationId(Guid conversationId)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationsId == conversationId)
                .ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                // Không có tin nhắn nào trong cuộc trò chuyện
                return false;
            }

            _context.Messages.RemoveRange(messages);
            await _context.SaveChangesAsync();

            return true;
        }
      
    }
}
