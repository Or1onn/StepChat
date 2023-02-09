using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ChatUserModel
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public int UserId { get; set; }
}
