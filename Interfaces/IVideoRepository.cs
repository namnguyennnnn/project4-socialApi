using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IVideoRepository
    {
        Task<Video> GetVideoByIdPostAsync(Guid postId);

        Task CreateVideoAsync(Video video);

        Task<Video> GetVideoByIdAsync(Guid videoId);

        Task RemoveVideoAsync(Guid videoId);

        Task<int> SaveChangeAsync();
    }
}
