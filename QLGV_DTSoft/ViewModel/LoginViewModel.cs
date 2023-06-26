using QLGV_DTSoft.Data;

namespace QLGV_DTSoft.ViewModel
{
    public class LoginViewModel
    {
        public string Tennguoidung { get; set; }

        public string Matkhau { get; set; }

        public string ReturnUrl { get; set; }

        public NguoiDung? User { get; set; }
    }
}
