using System.ComponentModel.DataAnnotations;

namespace StepChat.Models
{
    public class GroupsModel
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
