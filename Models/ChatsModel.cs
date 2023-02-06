using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ChatsModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ChatsStorageModel IdNavigation { get; set; } = null!;

    public virtual MessagesModel? Message { get; set; }
}
