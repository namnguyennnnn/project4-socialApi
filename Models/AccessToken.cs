using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAn4.Models
{
    public class AccessToken
    {
        [Key]
        public Guid AccessTokenId { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public Guid UserId { get; set; }

        // thêm trường UserId để liên kết với bảng User
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
