using System.ComponentModel.DataAnnotations;

namespace StepChat.Models
{
    public class TeachersModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public string? Patronymic { get; set; }
        public string? TeachingGroups { get; set; }
        public string? Email { get; set; }
    }
}
