
using DoAn4.Services.FriendshipService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
       

        public FriendController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
           
        }

        [HttpPost("add-friendships"),Authorize]
        public async Task<IActionResult> CreateFriendshipRequest( Guid friendUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.SendFriendRequest(token, friendUserId);
                if (!result)
                {
                    return BadRequest("Hai người đã là bạn hoặc yêu cầu kết bạn đã tồn tại");
                }

                return Ok("Gửi lời mời kết bạn thành công");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpPut("accept-friendships/{senderId}"), Authorize]
        public async Task<IActionResult> AcceptFriendshipRequest( Guid senderId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.AcceptFriendRequest(token, senderId);
                if (!result)
                {
                    return BadRequest();
                }

                return Ok("Đã chấp nhận lời mời kết bạn");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("reject-friendships/{senderId}"),Authorize]
        public async Task<IActionResult> RejectFriendshipRequest(Guid senderId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.RejectFriendRequest(token, senderId);
                if (!result)
                {
                    return BadRequest();
                }

                return Ok("Đã từ chối lời mời kết bạn");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpGet("listPending-friendships"),Authorize]
        public async Task<IActionResult> GetListFriendshipPending()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.GetListFriendshipPendingAsync(token);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpGet("list-friendships"),Authorize]
        public async Task<IActionResult> GetListFriendship()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.GetListFriendshipAsync(token);
                return Ok(result);           
                
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("remove-friendships/{friendsUserId}"), Authorize]
        public async Task<IActionResult> DeleteFriendship(Guid friendsUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.DeleteFriendship(token, friendsUserId);
                if (!result)
                {
                    return BadRequest();
                }

                return Ok(" Hủy kết bạn thành công");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("revoke-friendrequest/{friendsUserId}"), Authorize]
        public async Task<IActionResult> RevokeFriendRequest(Guid friendsUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.DeleteFriendship(token, friendsUserId);
                if (!result)
                {
                    return BadRequest();
                }

                return Ok(" Hủy kết bạn thành công");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

    }
}
