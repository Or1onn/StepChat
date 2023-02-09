using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class MessagesStatusModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool IsRead { get; set; }
}
