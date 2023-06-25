using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;

namespace QLGV_DTSoft.Controllers
{
    [Authorize]
    public class KetxuatController : Controller
    {
        private readonly DtsoftContext _context;

        public KetxuatController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: Ketxuat
        public async Task<IActionResult> Index(int? idbp)
        {
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;

            if (khuvucId != null)
            {
                var bp = _context.BoPhans.Where(b => b.IdKhuvuc == khuvucId).ToList();
                bp.Insert(0, new BoPhan { IdBp = 0, Tenbophan = "------------Tất Cả------------" });
                ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);
                if (idbp != 0 && idbp != null)
                {
                    string tenBoPhan = bp.FirstOrDefault(b => b.IdBp == idbp)?.Tenbophan;
                    ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);
                    ViewBag.tbkx = "Danh sách nhân viên theo bộ phận: " + tenBoPhan;
                    var dtsoftContext2 = _context.NguoiDungs.Include(n => n.IdBpNavigation).Include(n => n.IdVtNavigation).Where(h => h.IdBp == idbp);
                    return View(await dtsoftContext2.ToListAsync());
                }
            }
            var dtsoftContext = _context.NguoiDungs.Include(n => n.IdBpNavigation).Include(n => n.IdVtNavigation);
            return View(await dtsoftContext.ToListAsync());
        }

        // GET: Ketxuat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NguoiDungs == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.IdBpNavigation)
                .Include(n => n.IdVtNavigation)
                .FirstOrDefaultAsync(m => m.IdNd == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: Ketxuat/Create
    }
}
