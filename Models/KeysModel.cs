using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class KeysModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Key1 { get; set; } = null!;

    public virtual PrivateKeysStorageModel IdNavigation { get; set; } = null!;
}
