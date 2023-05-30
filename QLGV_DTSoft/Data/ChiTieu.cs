using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class ChiTieu
{
    public int IdCt { get; set; }

    public int IdKh { get; set; }

    public string? Tenchitieu { get; set; }

    public string? Chitieu1 { get; set; }

    public string? Motact { get; set; }

    public virtual KeHoachGiaoViec IdKhNavigation { get; set; } = null!;
}
