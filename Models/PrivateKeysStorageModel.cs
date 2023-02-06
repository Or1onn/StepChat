using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class PrivateKeysStorageModel
{
    public int Id { get; set; }

    public int KeysId { get; set; }

    public virtual UsersModel IdNavigation { get; set; } = null!;

    public virtual KeysModel? Key { get; set; }
}
