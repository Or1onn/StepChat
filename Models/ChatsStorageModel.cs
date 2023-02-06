using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ChatsStorageModel
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public virtual ChatsModel? Chat { get; set; }
}
