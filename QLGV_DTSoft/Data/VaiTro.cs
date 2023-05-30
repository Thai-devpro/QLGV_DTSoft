using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class VaiTro
{
    public int IdVt { get; set; }

    public string? Tenvaitro { get; set; }

    public string? Mota { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<Quyen> IdQuyens { get; set; } = new List<Quyen>();
}
