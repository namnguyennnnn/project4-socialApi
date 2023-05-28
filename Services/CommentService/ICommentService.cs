using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Services.CommentService
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllCommentAsync(Guid postId);
        Task<Comment> CreateCommentAsync(string token , Guid postId, string content);
        Task<bool> UpdateCommentAsync(string token, Guid commentId, string content);
        Task<bool> DeleteCommentAsync(string token,Guid postId,Guid commentId);
    }
}
