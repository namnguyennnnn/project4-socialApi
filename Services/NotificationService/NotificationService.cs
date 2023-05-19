using DoAn4.Interfaces;
using DoAn4.Models;
using System.Globalization;

namespace DoAn4.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly string formatCurentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                                                    .ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

        private readonly INotifyRepository _notifyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
       

        public NotificationService( INotifyRepository notifyRepository, IUserRepository userRepository ,IFriendshipRepository friendshipRepository)
        {
            _notifyRepository = notifyRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;  
        }

       

        public async Task SendFriendRequestNotification(Guid sender, Guid receiver)
        {
            var senderInfo = await _userRepository.GetUserByIdAsync(sender);
            var idfriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(senderInfo.UserId, receiver);
            var notification = new Notify
            {
                NotifyId  = Guid.NewGuid(),
                NotifyContent = $"{senderInfo.Fullname} đã gửi cho bạn một lời mời kết bạn",
                NotifyTime = DateTime.ParseExact(formatCurentTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                NotifyType = "friend_request",
                IsRead = false,
                FriendShipId = idfriendship.FriendshipId
            };

            await _notifyRepository.CreateNotifyAsync(notification);
        }

        public async Task AcceptFriendRequestNotification(Guid receiver, Guid sender)
        {
            var senderInfo = await _userRepository.GetUserByIdAsync(receiver);
            var idfriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(receiver, sender);
            var notification = new Notify
            {
                NotifyId = Guid.NewGuid(),
                NotifyContent = $"{senderInfo.Fullname} đã chấp nhận lời mời kết bạn",
                NotifyTime = DateTime.ParseExact(formatCurentTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                NotifyType = "acp_friend_request",
                IsRead = false,
                FriendShipId = idfriendship.FriendshipId
            };
            await _notifyRepository.CreateNotifyAsync(notification);
        }

    }
}
