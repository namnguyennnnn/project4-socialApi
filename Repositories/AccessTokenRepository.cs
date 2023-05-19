using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Migrations;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly DataContext _context;

        public AccessTokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddAccessTokenAsync(AccessToken accessToken)
        {
            await _context.AccessTokens.AddAsync(accessToken);
            await _context.SaveChangesAsync();
        }

        public async Task<AccessToken> GetAccessTokenAsync(string token)
        {
            return await _context.AccessTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateAccessTokenAsync(AccessToken accessToken)
        {
            _context.AccessTokens.Update(accessToken);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccessTokenAsync(string token)
        {
            var delAccessToken = await _context.AccessTokens.FirstOrDefaultAsync(at => at.Token == token);
            if (delAccessToken != null)
            {
                _context.AccessTokens.Remove(delAccessToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
