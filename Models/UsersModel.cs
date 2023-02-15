using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class UsersModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int ImageId { get; set; } = 0;

    public int PrivateKeysStorageId { get; set; }

    public string? Role { get; set; } = "Student";
}
