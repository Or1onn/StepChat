using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ChatsModel
{
    public int Id { get; set; }

    public int ChatId { get; set; }
    public string? Name { get; set; }

    public int CreateChatUserId { get; set; }
}
