using DoAn4.DTOs;
using DoAn4.DTOs.UserDTO;


namespace DoAn4.Services.FriendshipService
{
    public interface IFriendshipService
    {
        Task<List<InfoUserDTO>> GetListFriendshipPendingAsync(string token);

        Task<List<InfoUserDTO>> GetListFriendshipAsync(string token);

        

        Task<bool> DeleteFriendship(string token,Guid friendUserId);

        Task<bool> SendFriendRequest(string token , Guid frienduserid);

        Task<bool> AcceptFriendRequest(string token, Guid frienduserid);

        Task<bool> RejectFriendRequest(string token, Guid frienduserid);

    }
}
