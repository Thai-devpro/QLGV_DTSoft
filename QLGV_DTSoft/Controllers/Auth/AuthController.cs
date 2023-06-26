using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.ViewModel;
using System.Security.Claims;
using System.Security.Cryptography;
using QLGV_DTSoft.Helper;
using Microsoft.AspNetCore.Http;

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
                    loginViewModel.User = await GetUser(loginViewModel.Tennguoidung);
                    // Tạo identity và cookie authentication
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginViewModel.User.Hoten)
                    ,new Claim("idNguoidung", loginViewModel.User.IdNd.ToString())
                    ,new Claim(ClaimTypes.Role, loginViewModel.User.IdVtNavigation.Tenvaitro)
                    ,new Claim("idvaitro", loginViewModel.User.IdVt.ToString())
                    ,new Claim("idBophan", loginViewModel.User.IdBp.ToString())
                    ,new Claim("tenBophan", loginViewModel.User.IdBpNavigation.Tenbophan)
                    ,new Claim("idKhuvuc",  loginViewModel.User.IdBpNavigation.IdKhuvuc.ToString())
                    ,new Claim("tenKhuvuc", loginViewModel.User.IdBpNavigation.IdKhuvucNavigation.Tenkhuvuc)
                    
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

        private async Task<NguoiDung> GetUser(string tennguoidung)
        {
            return await _context.NguoiDungs.Include(u => u.IdBpNavigation).ThenInclude(uu => uu.IdKhuvucNavigation).Include(u => u.IdVtNavigation).FirstOrDefaultAsync(u => u.Tennguoidung == tennguoidung);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
