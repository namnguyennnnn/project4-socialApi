namespace DoAn4.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendFriendRequestNotification(Guid sender, Guid receiver);
        Task AcceptFriendRequestNotification(Guid receiver , Guid sender);
        
    }
}
