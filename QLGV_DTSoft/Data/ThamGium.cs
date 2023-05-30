using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class ThamGium
{
    public int IdNd { get; set; }

    public int IdKh { get; set; }

    public int? SlHoanthanh { get; set; }

    public string? Danhgia { get; set; }

    public virtual KeHoachGiaoViec IdKhNavigation { get; set; } = null!;

    public virtual NguoiDung IdNdNavigation { get; set; } = null!;
}
