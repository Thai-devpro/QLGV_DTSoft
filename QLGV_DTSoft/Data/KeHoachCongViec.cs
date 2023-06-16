using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class KeHoachCongViec
{
    public int IdKhcv { get; set; }

    public DateTime Namthuchien { get; set; }
    public string NamthuchienFormatted
    {
        get { return Namthuchien.ToString("yyyy"); }
    }
    public string? Noidungcongviec { get; set; }

    public virtual ICollection<KeHoachGiaoViec> KeHoachGiaoViecs { get; set; } = new List<KeHoachGiaoViec>();
}
