using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    [Table("passwords")]
    public class Password
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("hashed_password")]
        public string HashedPassword { get; set; } = string.Empty;

        [Column("salt")]
        public string? Salt { get; set; }

        [Column("last_changed")]
        public DateTime LastChanged { get; set; } = DateTime.UtcNow;
    }
}
