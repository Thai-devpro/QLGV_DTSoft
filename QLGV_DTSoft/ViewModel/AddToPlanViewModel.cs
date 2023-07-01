using QLGV_DTSoft.Data;

namespace QLGV_DTSoft.ViewModel
{
    public class AddToPlanViewModel
    {
        public KeHoachGiaoViec? KeHoachGiaoViec { get; set; }
        public IEnumerable<NguoiDung>? DsNhanvien { get; set; }

        public List<ChiTieu>? ChiTieu { get; set; }
    }
}
