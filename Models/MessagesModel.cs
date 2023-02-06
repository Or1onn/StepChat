using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class MessagesModel
{
    public int Id { get; set; }

    public string User { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime Time { get; set; }

    public virtual ChatsModel IdNavigation { get; set; } = null!;
}
