using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IImageRepository
    {
        Task<Images> GetImageByIdPostAsync(Guid postId);

        Task<Images> GetImageByIdAsync(Guid imageId);

        Task CreateImageAsync(Images images);

        Task<List<Images>> GetRemovedImagesFromPost(Post post, List<string>? Images);

        Task RemoveImageAsync(Guid imageId);

        Task<int> SaveChangeAsync();
    }
}
