using AutoMapper;
using DoAn4.Data;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;

using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public UserRepository(IWebHostEnvironment env,DataContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
            _environment = env;
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && !string.IsNullOrEmpty(user.Avatar))
            {
                user.Avatar = Path.Combine(_environment.ContentRootPath, user.Avatar);
                user.CoverPhoto = Path.Combine(_environment.ContentRootPath, user.CoverPhoto);
            }
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Avatar))
            {
                user.Avatar = Path.Combine(_environment.ContentRootPath, user.Avatar);
                user.CoverPhoto = Path.Combine(_environment.ContentRootPath, user.CoverPhoto);
            }
            return user;
        }

        
        public async Task<List<InfoUserDTO>> GetListUserAsync(List<Guid> UserIds)
        {
            var users = await _context.Users.Where(u => UserIds.Contains(u.UserId)).ToListAsync();
            var result = _mapper.Map<List<InfoUserDTO>>(users);
            result.ForEach(u => u.Avatar = Path.Combine(_environment.ContentRootPath, u.Avatar));
            return result;
        }
        

        public async Task UpdateUserAsync(User user)
        {     
            _context.Users.Update(user);
            await _context.SaveChangesAsync();                 
        }
       

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}
