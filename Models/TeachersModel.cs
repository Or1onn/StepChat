using System.ComponentModel.DataAnnotations;

namespace StepChat.Models
{
    public class TeachersModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string TeachingGroups { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
