namespace StepChat.Models
{
    public class MessagesModel
    {
        public string?  Text { get; set; }
        public DateTime Time { get; set; }
        public byte[]? IV { get; set; }
    }
}
