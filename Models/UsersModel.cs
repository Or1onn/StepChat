using System.ComponentModel.DataAnnotations;

namespace StepChat.Models
{
    public class UsersModel
    {
        [Key]
        public int Id { get; set; }

        public string? Email { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public string? FullName { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public string? PhoneNumber { get; set; } = null!;

        public int ImageId { get; set; }

        public byte[] JWT { get; set; } = null!;
    }
}
