using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class KeysModel
{
    public int Id { get; set; }

    public int KeyOwnerId { get; set; }

    public string Email { get; set; } = null!;

    public string Key { get; set; } = null!;
}
