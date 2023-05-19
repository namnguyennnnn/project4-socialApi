using DoAn4.DTOs;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;

namespace DoAn4.Services.LikeService
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;

        public LikeService(IUserRepository userRepository, IAuthenticationService authenticationService, IPostRepository postRepository  , ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
        }

        public async Task<bool> UpdatePostLikeStatusAsync( string token,Guid postId, LikeDto request)
        {
            var userId = await _authenticationService.GetIdUserFromAccessToken(token);           
            var like = await _likeRepository.GetLike(postId, userId.UserId);
            if (like != null)
            {
                if (request.React == 5)
                {                   
                    await _likeRepository.DeleteLike(like);
                }
                else
                {
                    
                    like.React = request.React;
                    await _likeRepository.UpdateLike(like);
                }
            }
            else
            {
                if (request.React == 5)
                {
                    
                     throw new Exception( "không thể unlike bài biết chưa like");
                    
                }
             
                like = new Like
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId.UserId,
                    React = request.React,
                    
                };
                await _likeRepository.CreateLike(like);
            }

            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("không tìm thấy bài đăng");
            }

            if (request.React == 0 && like != null)
            {
                
                post.TotalReact -= 1;
            }
            else if (request.React != 0)
            {
                
                post.TotalReact += 1;
            }

            await _postRepository.UpdatePostAsync(post);         
            return true;
        }

        public async Task<List<InfoUserDTO>> GetInfoUsersLikedAsync(Guid postId)
        {
            List<Guid> listIdUser = await _likeRepository.GetIdsUserLikedAsync(postId);
            if (listIdUser == null)
            {
                throw new ArgumentException("không có ai thích bài");
            }

            List<InfoUserDTO> usersInfo = await _userRepository.GetListUserAsync(listIdUser);
            if (usersInfo == null)
            {
                throw new ArgumentException("Không lấy được danh sách người thích bài");
            }
            return usersInfo;
        }
    }
}
