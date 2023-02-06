namespace StepChat.Models
{
    public class MessagesModel
    {
        public int Id { get; set; }

        public string ChatId { get; set; } = null!;

        public string User { get; set; } = null!;

        public string Text { get; set; } = null!;
    }
}
