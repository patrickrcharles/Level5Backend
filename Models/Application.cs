using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

public partial class Application
{
    public int Id { get; set; }

    public string CurrentVersion { get; set; } = null!;
}
