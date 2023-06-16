using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class CoQuyenTruyCap
{
    public int IdQuyentc { get; set; }

    public int IdVt { get; set; }

    public int IdQuyen { get; set; }

    public virtual Quyen IdQuyenNavigation { get; set; } = null!;

    public virtual VaiTro IdVtNavigation { get; set; } = null!;
}
