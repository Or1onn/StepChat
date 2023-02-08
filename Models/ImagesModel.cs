using System;
using System.Collections.Generic;

namespace StepChat.Models;

public partial class ImagesModel
{
    public int Id { get; set; }

    public byte[] Image { get; set; } = null!;
}
