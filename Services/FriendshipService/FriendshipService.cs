using DoAn4.DTOs;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.NotificationService;
using Mailjet.Client.Resources;
using System.Security.Authentication;

namespace DoAn4.Services.FriendshipService
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuthenticationService _authenticationService;

        public FriendshipService(IFriendshipRepository friendshipRepository, IUserRepository userRepository, INotificationService notificationService, IAuthenticationService authenticationService)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _authenticationService = authenticationService;
        }


        public async Task<List<InfoUserDTO>> GetListFriendshipPendingAsync(string token)
        {
            var sender = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new AuthenticationException("Token đã hết hạn");

            List<FriendshipDto> listfriendships = await _friendshipRepository.GetAllFriendshipPendingAsync(sender.UserId);

            if (listfriendships == null)
            {
                throw new ArgumentNullException(nameof(listfriendships), "Không có người nào muốn làm bạn với mày cả");
            }

            List<Guid> friendshipUserIds = await _friendshipRepository.GetAllFriendUseridFromListFriendshipAsync(listfriendships) ?? throw new Exception("Có lỗi xảy ra khi lấy danh sách bạn bè");

            List<InfoUserDTO> friendPendingOfUser = await _userRepository.GetListUserAsync(friendshipUserIds);
            return friendPendingOfUser;
        }

        public async Task<List<InfoUserDTO>> GetListFriendshipAsync(string token)
        {
            var sender = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new AuthenticationException("Token đã hết hạn");

            List<FriendshipDto> listfriendships= await _friendshipRepository.GetAllFriendshipAsync(sender.UserId);

            if(listfriendships == null)
            {
                throw new ArgumentNullException(nameof(listfriendships), "Không có người bạn nào");
            }

            List<Guid> friendshipUserIds = await _friendshipRepository.GetAllFriendUseridFromListFriendshipAsync(listfriendships) ?? throw new Exception("Có lỗi xảy ra khi lấy danh sách bạn bè");

            List<InfoUserDTO> friendOfUser = await _userRepository.GetListUserAsync(friendshipUserIds);

            return friendOfUser;
        }

        public async Task<bool> SendFriendRequest(string token, Guid friendUserId)
        {

            var sender = await _authenticationService.GetIdUserFromAccessToken(token);
            var receiver = await _userRepository.GetUserByIdAsync(friendUserId);

            if (sender == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (sender == null || receiver == null)
            {
                throw new AuthenticationException("Người dùng không tồn tại");
            }
            else if (await _friendshipRepository.IsFriendship(sender.UserId, friendUserId))
            {
                throw new Exception("Hai người đã là bạn ");
            }
            else if (await _friendshipRepository.IsFriendshipRequestExit(sender.UserId, friendUserId) == false) 
            {
                throw new Exception("Đã tồn tại yêu cầu kết bạn ");
            }

            var friendship = new Friendship
            {
                FriendshipId = Guid.NewGuid(),
                UserId = sender.UserId,
                FriendUserId = friendUserId,
                FriendStatus = 0
            };

            await _friendshipRepository.AddFriendshipAsync(friendship);

            await _notificationService.SendFriendRequestNotification(sender.UserId, friendUserId);

            return true;
        }

        public async Task<bool> AcceptFriendRequest(string token, Guid friendUserId)
        {

            var sender = await _authenticationService.GetIdUserFromAccessToken(token);
            var receiver = await _userRepository.GetUserByIdAsync(friendUserId);

            if (sender == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (sender == null || receiver == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            else if (await _friendshipRepository.IsFriendship(sender.UserId, friendUserId))
            {
                throw new Exception("Hai người đã là bạn bè");
            }

            await _friendshipRepository.AcceptFriendAsync(sender.UserId, friendUserId);

            await _notificationService.AcceptFriendRequestNotification(friendUserId, sender.UserId );
            return true;
        }
         
        public async Task<bool> DeleteFriendship(string token, Guid friendUserId)
        {
            var curentUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var idFriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId( curentUser.UserId , friendUserId);
            if (curentUser == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (idFriendship == null) 
            {
                var idFriendship2 = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(friendUserId, curentUser.UserId);
                await _friendshipRepository.DeleteFriendshipAsync(idFriendship2.FriendshipId);
                return true;

            }
            await _friendshipRepository.DeleteFriendshipAsync(idFriendship.FriendshipId);
            return true;
        }

        public async Task<bool> RejectFriendRequest(string token, Guid senderId)
        {
            var receiver = await _authenticationService.GetIdUserFromAccessToken(token);
            var sender = await _userRepository.GetUserByIdAsync(senderId);
            var idFriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(receiver.UserId, senderId);
            if (sender == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (sender == null || receiver == null)
            {
                throw new AuthenticationException("Người dùng không tồn tại");
            }

            else if (await _friendshipRepository.IsFriendship(receiver.UserId, senderId))
            {
                throw new AuthenticationException("Hai đã là bạn bè");
            }
            
            await _friendshipRepository.DeleteFriendshipAsync(idFriendship.FriendshipId);  
            return true;
        }

       
    }
}
