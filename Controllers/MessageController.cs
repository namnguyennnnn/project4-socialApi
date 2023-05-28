
using DoAn4.Services.MessageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send-message"), Authorize]
        public async Task<ActionResult> SendMessage([FromBody]Guid recipientId, string content)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _messageService.SendMessage(token, recipientId, content);

            if (result == null)
            {
                return BadRequest("Gửi tin nhắn thất bại");
            }
            return Ok(result);
        }

        [HttpGet("get-message"), Authorize]
        public async Task<ActionResult> getMessage( Guid conversationId)
        {          
            var result = await _messageService.GetMessages(conversationId);

            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    } 
}
