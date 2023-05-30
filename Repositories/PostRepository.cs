using AutoMapper;
using DoAn4.Data;
using DoAn4.DTOs;
using DoAn4.DTOs.PostDTO;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;


namespace DoAn4.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostRepository(DataContext context , IWebHostEnvironment env) 
        {
            _context = context;
            _environment = env;
        }

        public async Task<List<InFoPostDto>> GetNewFeeds(List<Guid> friendIds,Guid curentUserId, int skip, int take)
        {
            var posts = await _context.Posts
             .Include(p => p.Images)
             .Include(p => p.Videos)
             .Where(p => (friendIds.Contains(p.UserPostId) || p.UserPostId == curentUserId) && !p.IsDeleted)
             .OrderByDescending(p => p.PostTime)
             .Skip(skip)
             .Take(take)
             .Select(p => new InFoPostDto
             {
                 PostId = p.PostId,
                 Content = p.Content,
                 TotalReact = p.TotalReact,
                 TotalComment = p.TotalComment,
                 PostTime = p.PostTime,
                 UpdateTime = p.UpdateTime,
                 User = new InfoUserDTO
                 {
                     UserId = p.User.UserId,
                     Email = p.User.Email,
                     Fullname = p.User.Fullname,
                     Gender = p.User.Gender,
                     Avatar = p.User.Avatar
                 },
                 Images = p.Images.Select(i => Path.Combine(_environment.ContentRootPath, i.ImageLink)).ToList(),
                 Videos = p.Videos.Select(v => Path.Combine(_environment.ContentRootPath, v.VideoLink)).ToList()
             })
             .ToListAsync();
            return posts;
        }

        public async Task<Guid> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post.PostId;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<List<Post>> GetAllPostByIdUserAsync(Guid userId)
        {
            var listPost = await _context.Posts.Where(p => p.UserPostId == userId).ToListAsync();
            return listPost;
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePostAsync(Guid PostId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == PostId);
            if (post != null)
            {
                post.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
    }
}
