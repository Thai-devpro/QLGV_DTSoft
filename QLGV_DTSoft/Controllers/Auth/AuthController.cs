using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.ViewModel;
using System.Security.Claims;
using System.Security.Cryptography;
using QLGV_DTSoft.Helper;

namespace QLGV_DTSoft.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly DtsoftContext _context;
        public AuthController(DtsoftContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string ReturnUrl = "")
        {
            LoginViewModel objLoginModel = new LoginViewModel();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập
                bool isValid = await IsUserValid(loginViewModel.Tennguoidung, loginViewModel.Matkhau);

                if (isValid)
                {
                    // Tạo identity và cookie authentication
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginViewModel.Tennguoidung)
                    // Các claim khác có thể được thêm vào đây
                };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // Cấu hình thuộc tính của cookie authentication (nếu cần thiết)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Tennguoidung == loginViewModel.Tennguoidung);

                    HttpContext.Session.SetString("tennd", user.Hoten);
                    HttpContext.Session.SetInt32("idnd", user.IdNd);
                    HttpContext.Session.SetInt32("vaitro", user.IdVt);
                    return LocalRedirect(loginViewModel.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
                }

            }
            return View();
        }

        private async Task<bool> IsUserValid(string username, string password)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Tennguoidung == username);

            if (user != null)
            {
                if (SecretHasher.Verify(password, user.Matkhau))
                {
                    return true;
                }
            }

            return false; 
        }

        public async Task<IActionResult> LogOut()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return LocalRedirect("/");
        }

        
    }
}
