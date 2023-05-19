
using DoAn4.DTOs.PostDTO;
using DoAn4.Models;


namespace DoAn4.Services.PostService
{
    public interface IPostService
    {
        Task<List<InFoPostDto>> GetFriendPostsAsync(string token, int skip, int take);
        Task<Post> CreatePostAsync(string token, PostDto? postDto =null);
        Task<bool> UpdatePostAsync(string token,Guid postId,UpdatePostDto? filedto=null);
        Task<bool> DletePostAsync(string token, Guid posdId);

        Task<string> UpdateAvatarAsync(string token, IFormFile ImageFile);

    }
}
