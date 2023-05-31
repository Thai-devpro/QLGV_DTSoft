﻿using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class KeHoachGiaoViec
{
    public int IdKh { get; set; }

    public int IdBp { get; set; }

    public int IdKhcv { get; set; }

    public string? Tenkehoach { get; set; }

    public DateTime? Ngaybatdau { get; set; }

    public DateTime? Ngayketthuc { get; set; }

    public string? Motakh { get; set; }

    public byte[]? Ngaytaokh { get; set; }

    public virtual ICollection<ChiTieu> ChiTieus { get; set; } = new List<ChiTieu>();

    public virtual BoPhan IdBpNavigation { get; set; } = null!;

    public virtual KeHoachCongViec IdKhcvNavigation { get; set; } = null!;

    public virtual ICollection<ThamGium> ThamGia { get; set; } = new List<ThamGium>();
}