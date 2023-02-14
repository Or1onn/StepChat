using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class KeysModel
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public string Key { get; set; } = null!;
}
