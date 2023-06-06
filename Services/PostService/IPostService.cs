
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.PostDTO;
using DoAn4.Models;


namespace DoAn4.Services.PostService
{
    public interface IPostService
    {
        Task<List<InFoPostDto>> GetFriendPostsAsync(string token, int skip, int take);
        Task<InFoPostDto> CreatePostAsync(string token, PostDto? postDto =null);
        Task<InFoPostDto> UpdatePostAsync(string token,Guid postId,UpdatePostDto? filedto=null);
        Task<ResultRespone> DeletePostAsync(string token, Guid posdId);
        Task<List<InFoPostDto>> GetSelfPostsAsync(string token);
        Task<string> UpdateAvatarAsync(string token, IFormFile ImageFile);

    }
}
