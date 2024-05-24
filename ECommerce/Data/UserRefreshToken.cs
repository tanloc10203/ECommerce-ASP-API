using ECommerce.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data
{
    [Table("UserRefreshToken")]
    public class UserRefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]

        public ApplicationUser? User { get; set; }

        public string Token { get; set; }
        public string JwtId { get; set; }

        public bool IsRevoked { get; set; }

        public bool IsUsed { get; set; }

        // Ngày tạo
        public DateTime IssuedAt { get; set; }

        // Ngày hết hạn
        public DateTime ExpiredAt { get; set; }
    }
}
