using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Models;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace QLGV_DTSoft.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DtsoftContext _context;

        public HomeController(ILogger<HomeController> logger , DtsoftContext context)
        {
            _logger = logger;
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var nguoidungIdClaim = User.FindFirstValue("idnguoidung");
            if (!string.IsNullOrEmpty(nguoidungIdClaim))
            {
                int nguoidungId = int.Parse(nguoidungIdClaim);

                // Truy vấn danh sách công việc của người dùng theo ID
                var kehoachGiaoViec = await _context.KeHoachGiaoViecs
                                        .Include(u => u.IdBpNavigation)
                                        .ThenInclude(uu => uu.IdKhuvucNavigation)
                                        .Include(u => u.ThamGia)
                                        .Include(u => u.ChiTieus)
                                        .Where(kh => kh.ThamGia.Any(tg => tg.IdNd == nguoidungId))
                                        .ToListAsync();

                return View(kehoachGiaoViec);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTienDo(int idKeHoach, Dictionary<int, int> tienDoHoanThanh)
        {
            var nguoidungIdClaim = User.FindFirstValue("idNguoidung");
            if (!string.IsNullOrEmpty(nguoidungIdClaim))
            {
                int idNd = int.Parse(nguoidungIdClaim);
                foreach (var kvp in tienDoHoanThanh)
                {
                    int idCt = kvp.Key;
                    int slHoanthanh = kvp.Value;

                    var thamGia = await _context.ThamGia.FirstOrDefaultAsync(tg => tg.IdNd == idNd && tg.IdKh == idKeHoach && tg.IdCt == idCt);

                    if (thamGia != null)
                    {
                        thamGia.SlHoanthanh = (thamGia.SlHoanthanh ?? 0) + slHoanthanh;
                        await _context.SaveChangesAsync();
                        
                    }
                }
                return RedirectToAction("Index");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult norole()
        {
            
            return View();
        }
    }
}