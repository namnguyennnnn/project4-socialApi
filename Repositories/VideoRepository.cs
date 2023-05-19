using AutoMapper;
using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;

namespace DoAn4.Repositories
{

    public class VideoRepository : IVideoRepository
    {
        private readonly DataContext _context;
       

        public VideoRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task CreateVideoAsync(Video video)
        {
            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();
        }

        public async Task<Video> GetVideoByIdAsync(Guid videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            return video;
        }
        public async Task RemoveVideoAsync(Guid videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);

            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }
        }
        public Task<Video> GetVideoByIdPostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
    }
}
