using System;
using System.Collections.Generic;

namespace QLGV_DTSoft.Data;

public partial class NguoiDung
{
    public int IdNd { get; set; }

    public int IdVt { get; set; }

    public int IdBp { get; set; }

    public string? Tennguoidung { get; set; }

    public string? Matkhau { get; set; }

    public string? Hoten { get; set; }

    public DateTime? Ngaysinh { get; set; }

    public string? Gioitinh { get; set; }

    public string? Sodienthoai { get; set; }

    public string? Diachi { get; set; }

    public string? Email { get; set; }

    public virtual BoPhan? IdBpNavigation { get; set; }

    public virtual VaiTro? IdVtNavigation { get; set; }

    public virtual ICollection<ThamGium> ThamGia { get; set; } = new List<ThamGium>();
}
