using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.NotificationService;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ResultRespone> SendFriendRequest(string token, Guid friendUserId)
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
            else if (await _friendshipRepository.IsFriendshipExist(sender.UserId, friendUserId))
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
            try
            {
                await _friendshipRepository.AddFriendshipAsync(friendship);
            
                await _notificationService.SendFriendRequestNotification(sender.UserId, friendUserId);
                return new ResultRespone
                { 
                    Status = 200
                };
            }
            catch(DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
           

            
        }

        public async Task<ResultRespone> AcceptFriendRequest(string token, Guid friendShipId)
        {

            var receiver = await _authenticationService.GetIdUserFromAccessToken(token);
            var friendship = await _friendshipRepository.GetFriendshipById(friendShipId);
            var sender = friendship.FriendUserId == receiver.UserId ? friendship.UserId : friendship.FriendUserId;
            if (receiver == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (receiver == null || sender == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            else if (await _friendshipRepository.IsFriendship(friendShipId))
            {
                throw new Exception("Hai người đã là bạn bè");
            }
                     
            await _friendshipRepository.AcceptFriendAsync(receiver.UserId, sender);
            await _notificationService.AcceptFriendRequestNotification(receiver.UserId,friendShipId);
            
           
            return new ResultRespone
            {
                Status = 200
            };
        }
         
        public async Task<ResultRespone> DeleteFriendship(string token, Guid friendUserId)
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
                return new ResultRespone
                {
                    Status = 200
                };

            }
            await _friendshipRepository.DeleteFriendshipAsync(idFriendship.FriendshipId);
            return new ResultRespone
            {
                Status = 200
            };
        }

        public async Task<ResultRespone> RejectFriendRequest( Guid friendShipId)
        {                   
            var Friendship = await _friendshipRepository.GetFriendshipById(friendShipId);
            if (Friendship == null)
            {
                throw new AuthenticationException("Hai người chả có quan hệ gì cả");
            }
            try 
            { 
                await _friendshipRepository.DeleteFriendshipAsync(friendShipId);
                return new ResultRespone
                {
                    Status = 200
                };
            }
            catch(DbUpdateException e) 
            {
                throw new Exception(e.Message);
            }                   
        }

       
    }
}
