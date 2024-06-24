using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

public partial class UserReport
{
    public int Id { get; set; }

    public string Report { get; set; } = null!;

    public int Userid { get; set; }

    public string UserName { get; set; } = null!;

    public string Os { get; set; } = null!;

    public string Device { get; set; } = null!;

    public string DeviceName { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string Ipaddress { get; set; } = null!;

    public DateTime Date { get; set; }
}
