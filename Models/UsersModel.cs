using System.ComponentModel.DataAnnotations;

namespace StepChat.Models
{
    public class UsersModel
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
