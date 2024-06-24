using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

public partial class ServerMessage
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? Date { get; set; }
}
