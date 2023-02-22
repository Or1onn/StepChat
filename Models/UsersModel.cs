using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StepChat.Models;

public partial class UsersModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;
    [RegularExpression(@"^\+994(?:50|51|55|70|77)\d{7}$|^\+994\d{2}\d{7}$", ErrorMessage = "Incorrect phone number")]
    public string PhoneNumber { get; set; } = null!;

    public int ImageId { get; set; } = 0;

    public int PrivateKeysStorageId { get; set; }

    public string? Role { get; set; } = "Student";
}
