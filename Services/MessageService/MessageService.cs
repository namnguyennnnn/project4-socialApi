using DoAn4.DTOs;
using DoAn4.Hubs;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using Microsoft.AspNetCore.SignalR;

namespace DoAn4.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IAuthenticationService _authenticationService;
        public MessageService(IAuthenticationService authenticationService, IMessageRepository messageRepository,IConversationRepository conversationRepository,IUserRepository userRepository, IHubContext<ChatHub> chatHubContext)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _chatHubContext = chatHubContext;
            _authenticationService = authenticationService;
        }
        public async Task<bool> SendMessage(string token, Guid recipientId, string content)
        {
            var sender = await _authenticationService.GetIdUserFromAccessToken(token);
            var recipient = await _userRepository.GetUserByIdAsync(recipientId);                             

            if (sender == null || recipient == null)
            {
                throw new Exception("Invalid sender or recipient.");
            }


            var conversation = await _conversationRepository.GetConversation(sender.UserId, recipientId);
            if (conversation == null)
            {
                conversation = await _conversationRepository.CreateConversation(sender.UserId, recipientId);
            }
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); 
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ConversationsId = conversation.ConversationsId,
                SenderId = sender.UserId,
                RecipientId = recipient.UserId,
                Content = content,
                CreatedAt = localTime
            };

            await _messageRepository.SendMessage(message);

            // Gửi tin nhắn tới client thông qua SignalR
            await _chatHubContext.Clients.Group(conversation.ConversationsId.ToString()).SendAsync("ReceiveMessage", message);

            return true;
        }

        public async Task<List<Message>> GetMessages(Guid conversationId)
        {
            return await _messageRepository.GetMessages(conversationId);
        }
    }
}
