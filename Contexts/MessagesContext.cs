namespace StepChat.Contexts
{
    public class MessagesContext
    {
        public string? Text { get; set; }
        public string? IV { get; set; }
        public string? PrivateKey { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
