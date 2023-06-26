using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class ChiTieu
{
    public int IdCt { get; set; }

    public int IdKh { get; set; }

    public string? Chitieu { get; set; }

    public int? Doanhso { get; set; }

    public string? Donvitinh { get; set; }

    public virtual KeHoachGiaoViec IdKhNavigation { get; set; } = null!;

    public virtual ICollection<ThamGium> ThamGia { get; set; } = new List<ThamGium>();
}
