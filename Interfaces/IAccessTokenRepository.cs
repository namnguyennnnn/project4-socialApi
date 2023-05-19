using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IAccessTokenRepository
    {
        Task UpdateAccessTokenAsync(AccessToken accessToken);

        Task<AccessToken> GetAccessTokenAsync(string token);

        Task AddAccessTokenAsync(AccessToken accessToken);

        Task DeleteAccessTokenAsync(string token);

        Task<int> SaveChangesAsync();
    }
}
