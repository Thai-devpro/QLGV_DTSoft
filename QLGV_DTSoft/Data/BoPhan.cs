using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class BoPhan
{
    public int IdBp { get; set; }

    public int IdKhuvuc { get; set; }

    public string? Tenbophan { get; set; }

    public string? Congviecchuyenmon { get; set; }

    public virtual KhuVuc IdKhuvucNavigation { get; set; } = null!;

    public virtual ICollection<KeHoachGiaoViec> KeHoachGiaoViecs { get; set; } = new List<KeHoachGiaoViec>();

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
