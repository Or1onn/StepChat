using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ChatsModel
{
    public int Id { get; set; }

    public string? User1Name { get; set; }
    public string? User2Name { get; set; }

    public TimeSpan? Time { get; set; }

    public int ChatId { get; set; }

    public int User1ImageId { get; set; } = 0;
    public int User2ImageId { get; set; } = 0;

    public int CreateChatUserId { get; set; }
}
