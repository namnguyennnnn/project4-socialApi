
using DoAn4.Models;
namespace DoAn4.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendFriendRequestNotification(Guid sender, Guid receiver);
        Task AcceptFriendRequestNotification( Guid curUser,Guid friendShipId);
        Task NotifyCommentPost(Guid postId, Guid commentatorId , Guid receiverNotify);
        Task<List<Notify>> GetAllNotifies(string token);

    }
}
