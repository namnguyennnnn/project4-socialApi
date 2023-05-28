﻿using DoAn4.DTOs;
using DoAn4.Interfaces;
using DoAn4.Models;

using DoAn4.Services.AuthenticationService;

using Microsoft.EntityFrameworkCore;


namespace DoAn4.Services.CommentService
{
    public class CommentService : ICommentService
    {
        

        private readonly ICommentRepository _commentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository, IAuthenticationService authenticationService)
        {
            _commentRepository = commentRepository;
            _authenticationService = authenticationService;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<List<CommentDto>> GetAllCommentAsync(Guid postId)
        {
            var comments = await _commentRepository.GetCommentsByPostId(postId);
            var commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var user = await _userRepository.GetUserByIdAsync(comment.UserId);
                var commentDto = new CommentDto
                {
                    Content = comment.Content,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    CommentTime = comment.CommentTime
                };
                commentDtos.Add(commentDto);
            }

            return commentDtos;
        }

        public async Task<Comment> CreateCommentAsync(string token, Guid postId,string content)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            if (user == null)
            {
                throw new Exception("Token hết hạn");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var newComment = new Comment
            {
                CommentId = Guid.NewGuid(),
                UserId = user.UserId,
                PostId = postId,
                Content = content,
                CommentTime = localTime,
                UpdateCommentTime = localTime,               
            };
            try
            {
                await _commentRepository.AddComment(newComment);
               
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                {
                    throw new ArgumentException("không tìm thấy bài đăng");
                }

                post.TotalComment += 1;

                await _postRepository.UpdatePostAsync(post);

                return newComment;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }
           
        }
        public async Task<bool> UpdateCommentAsync(string token, Guid commentId, string content)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var comment = await _commentRepository.GetCommentById(commentId);
            if(comment == null)
            {
                throw new Exception("Comment not found");
            }
            else if (user== null)
            {
                throw new ArgumentException("Token hết hạn");
            }
            else if(user.UserId!= comment.UserId)
            {
                throw new Exception("Bạn không phải chủ bình luận này");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            comment.Content = content;
            comment.CommentTime =localTime;
            comment.UpdateCommentTime =localTime;
            try
            {
                await _commentRepository.UpdateComment(comment);
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }

        }

        public async Task<bool> DeleteCommentAsync(string token,Guid postId,Guid commentId)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var post = await _postRepository.GetPostByIdAsync(postId);
            var comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }           
            else if (user == null)
            {
                throw new Exception("Token hết hạn");
            }

            else if (comment.UserId == user.UserId || post.UserPostId == user.UserId)
            {
                await _commentRepository.DeleteComment(commentId);
                return true;
            }

            return false;
            
        }

    }

       
}

