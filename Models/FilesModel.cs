using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class FilesModel
{
    public int Id { get; set; }

    public byte[] File { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;
}
