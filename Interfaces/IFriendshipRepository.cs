﻿using DoAn4.DTOs;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Interfaces
{
    public interface IFriendshipRepository
    {
        Task<FriendshipDto> GetFriendshipById(Guid FrendShipId);

        Task<Friendship> GetFriendshipByUserIdAndFriendUserId(Guid Sender, Guid Receiver);

        //Task<List<User>> GetAllFriendshipAsync(Guid UserId);

        Task AddFriendshipAsync(Friendship friendship);

        Task<List<FriendshipDto>> GetAllFriendshipPendingAsync(Guid currentUserId);

        Task<List<FriendshipDto>> GetAllFriendshipAsync(Guid currentUserId);

        Task<List<Guid>> GetAllFriendUseridFromListFriendshipAsync(List<FriendshipDto> friendships);

        Task AcceptFriendAsync(Guid currentUserId, Guid senderId);

        Task<bool> DeleteFriendshipAsync(Guid friendshipId);

        Task<bool> IsFriendshipRequestExit(Guid userId, Guid friendUserId);

        Task<bool> IsFriendship(Guid userId, Guid friendUserId);

        Task<int> SaveChangesAsync();
        Task<List<Guid>> GetAllFriendIdsAsync(Guid currentUserId);
       
    }
}
