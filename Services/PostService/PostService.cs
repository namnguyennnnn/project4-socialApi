
using DoAn4.DTOs.PostDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.ImageService;
using DoAn4.Services.VideoService;

using Microsoft.EntityFrameworkCore;

using System.Security.Authentication;


namespace DoAn4.Services.PostService
{
    public class PostService : IPostService
    {
       
        private readonly IPostRepository _postRepository;
        private readonly IImageService _imageService;
        private readonly IVideoService _videoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IFriendshipRepository _friendshipRepository;
        


        public PostService(IUserRepository userRepository, IFriendshipRepository friendshipRepository, IPostRepository postRepository, IVideoService videoService, IImageService imageService, IAuthenticationService authenticationService)
        {
            _postRepository = postRepository;
            _imageService = imageService;
            _videoService = videoService;
            _authenticationService = authenticationService;
            _friendshipRepository = friendshipRepository;
            
        }

        public async Task<List<InFoPostDto>> GetFriendPostsAsync(string token, int skip, int take)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var allFriendship = await _friendshipRepository.GetAllFriendIdsAsync(user.UserId);
            var postsOfFriends = await _postRepository.GetNewFeeds(allFriendship,user.UserId, skip, take);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (allFriendship == null)
            {
                throw new ArgumentNullException(nameof(allFriendship),"Không có người bạn nào");
            }
            return postsOfFriends;

        }

        public async Task<Post> CreatePostAsync(string token, PostDto? postDto = null)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }

            if (postDto == null || (string.IsNullOrEmpty(postDto.Content) && (postDto.ImageFiles == null || !postDto.ImageFiles.Any()) && (postDto.VideoFiles == null || !postDto.VideoFiles.Any())))
            {
                throw new ArgumentException("Không có thông tin bài đăng được cung cấp.");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var post = new Post
            {
                PostId = Guid.NewGuid(),
                UserPostId = user.UserId,
                Content = postDto?.Content,
                PostTime = localTime,
                UpdateTime = localTime,
                TotalReact = 0,
                TotalComment = 0,
                IsDeleted = false
            };

            await _postRepository.CreatePostAsync(post);

            if (postDto?.ImageFiles != null && postDto.ImageFiles.Any())
            {
                await _imageService.UploadImages(post.PostId, postDto.ImageFiles);
            }

            if (postDto?.VideoFiles != null && postDto.VideoFiles.Any())
            {
                await _videoService.UploadVideo(post.PostId, postDto.VideoFiles);
            }

            return post;
        }

        public async Task<bool> UpdatePostAsync(string token, Guid postId, UpdatePostDto? updatePostDto =null)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new Exception("Token đã hết hạn");
            var post = await _postRepository.GetPostByIdAsync(postId) ?? throw new ArgumentNullException(nameof(postId), "Bài đăng không tồn tại");

            // Kiểm tra xem user hiện tại có quyền chỉnh sửa bài đăng hay không
            if (post.UserPostId != user.UserId)
            {
                throw new Exception("Bạn không có quyền chỉnh sửa bài đăng này");
            }
            //kiểm tra có sửa status hay không
            if (updatePostDto.Content != null) {
                post.Content = updatePostDto?.Content;
                post.UpdateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            }
            // Xóa ảnh từ post
            if(updatePostDto?.IdImagesRemove != null && updatePostDto.IdImagesRemove.Any())
            {
                var isImageRemove = await _imageService.RemoveImage(postId, updatePostDto);
                if (isImageRemove == false)
                {
                    throw new Exception("Xóa ảnh thất bại");
                }
            }

            // Check có ảnh nào mới được thêm vào post không
            if(updatePostDto?.NewImages != null && updatePostDto.NewImages.Any())
            {
                var addImage = await _imageService.UploadImages(post.PostId, updatePostDto.NewImages);
                if (addImage == null)
                {
                    throw new Exception("Thêm ảnh thất bại");
                }
               
            }

            // Xóa video từ post
            if(updatePostDto?.IdVideosRemove != null && updatePostDto.IdVideosRemove.Any())
            {
                var isVideosRemove = await _videoService.RemoveVideo(postId, updatePostDto);
                if (isVideosRemove == false)
                {
                    throw new Exception("Xóa video thất bại");
                }
            }

            // Check có video nào mới được thêm vào post không
            if(updatePostDto?.Newvideos != null && updatePostDto.Newvideos.Any())
            {
                var addVideo = await _videoService.UploadVideo(post.PostId, updatePostDto.Newvideos);
                if (addVideo == null)
                {
                    throw new Exception("Thêm video thất bại");
                }
            }
            try
            {
                await _postRepository.UpdatePostAsync(post);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }

            return true;
        }
       
        public async Task<bool> DeletePostAsync(string token, Guid posdId)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var isDeletePost = await _postRepository.DeletePostAsync(posdId);

            if(user == null)
            {
                throw new Exception("Token hết hạn");
            }
            else if(isDeletePost != false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> UpdateAvatarAsync(string token, IFormFile imageFile)
        {
            var ext = Path.GetExtension(imageFile.FileName);
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (imageFile == null)
            {
                throw new ArgumentException("Không có thông tin bài đăng được cung cấp");
            }          
            else if(imageFile == null)
            {
                throw new ArgumentException("File ảnh chưa được cung cấp");
            }
            else {
                var post = new Post
                {
                    PostId = Guid.NewGuid(),
                    UserPostId = user.UserId,
                    Content = "Đã cập nhật 1 ảnh trên profile",
                    TotalReact = 0 ,
                    TotalComment = 0,
                    PostTime = localTime,
                    UpdateTime = localTime,
                    IsDeleted = false
                };

                await _postRepository.CreatePostAsync(post);


                var img = await _imageService.UploadImage(post.PostId, imageFile);
                var imgPath = img.ImageLink;

                return imgPath;
            }
            
        }

        public async Task<List<Post>> GetSelfPostsAsync(string token)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }

            var selfPosts = await _postRepository.GetAllPostByIdUserAsync(user.UserId);

            return selfPosts;
        }
    }

}