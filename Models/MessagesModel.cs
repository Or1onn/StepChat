using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class MessagesModel
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreateTime { get; set; } = DateTime.Now;
}
