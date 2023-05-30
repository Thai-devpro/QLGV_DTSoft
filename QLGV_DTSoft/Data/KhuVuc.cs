using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class KhuVuc
{
    public int IdKhuvuc { get; set; }

    public string? Tenkhuvuc { get; set; }

    public string? Diachi { get; set; }

    public string? Email { get; set; }

    public string? Sodienthoai { get; set; }

    public virtual ICollection<BoPhan> BoPhans { get; set; } = new List<BoPhan>();
}
