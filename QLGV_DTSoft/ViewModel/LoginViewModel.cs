using QLGV_DTSoft.Data;
using System.ComponentModel.DataAnnotations;

namespace QLGV_DTSoft.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Nhập tên người dùng")]
        public string Tennguoidung { get; set; }

        [Required(ErrorMessage ="Nhập mật khẩu")]
        public string Matkhau { get; set; }

        public string ReturnUrl { get; set; }

        public NguoiDung? User { get; set; }
    }
}
