using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class Quyen
{
    public int IdQuyen { get; set; }

    public string? Tenquyen { get; set; }

    public virtual ICollection<VaiTro> IdVts { get; set; } = new List<VaiTro>();
}
