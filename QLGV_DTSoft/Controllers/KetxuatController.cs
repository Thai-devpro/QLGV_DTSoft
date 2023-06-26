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
using QLGV_DTSoft.Helper;
using SautinSoft.Document;

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(8)]
    public class KetxuatController : Controller
    {
        private readonly DtsoftContext _context;

        public KetxuatController(DtsoftContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Export(string GridHtml)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "HTML");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string input = Path.Combine(path, "html1.html");
            string output = Path.Combine(path, "Danhsachnguoidung.docx");
            System.IO.File.WriteAllText(input, GridHtml);
            DocumentCore documentCore = DocumentCore.Load(input, new HtmlLoadOptions());
            documentCore.Save(output);
            byte[] bytes = System.IO.File.ReadAllBytes(output);

            Directory.Delete(path, true);

            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Danhsachnguoidung.docx");
        }
        // GET: Ketxuat
        public async Task<IActionResult> Index(int? idbp, int? idct, string? kq)
        {
            /*var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 8 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }*/

            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;

            if (khuvucId != null)
            {
                var bp = _context.BoPhans.Where(b => b.IdKhuvuc == khuvucId).ToList();
                bp.Insert(0, new BoPhan { IdBp = 0, Tenbophan = "------------Tất Cả------------" });
                ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);

                var ct = _context.ChiTieus.ToList();
                ct.Insert(0, new ChiTieu { IdCt = 0, Chitieu = "------------Tất Cả------------" });
                ViewBag.chitieu = new SelectList(ct, "IdCt", "Chitieu", idct);

                var distinctNth = new List<string>();
                distinctNth.Add("Tất cả");
                distinctNth.Add("Đạt");
                distinctNth.Add("Chưa đạt");
                distinctNth.Add("Không đạt");
                ViewBag.ketqua = new SelectList(distinctNth, kq);
                if (idbp == 0 && idbp != null && kq != null && kq != "Tất cả")
                {
                    string tenkhuvuc = _context.KhuVucs.FirstOrDefault(b => b.IdKhuvuc == khuvucId)?.Tenkhuvuc;
                    ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);
                    ViewBag.ketqua = new SelectList(distinctNth, kq);
                    ViewBag.tbkx = "Danh sách nhân viên " + kq.ToLower() + " kết quả theo khu vực: " + tenkhuvuc;
                    var users = _context.NguoiDungs
           .Where(nguoiDung => nguoiDung.IdBpNavigation.IdKhuvuc == khuvucId &&
                               nguoiDung.ThamGia.Any(thamGia => thamGia.Danhgia == kq));

                    return View(await users.ToListAsync());
                }
                if (idbp != 0 && idbp != null && kq != null && kq != "Tất cả")
                {
                    string tenBoPhan = bp.FirstOrDefault(b => b.IdBp == idbp)?.Tenbophan;
                    ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);
                    ViewBag.ketqua = new SelectList(distinctNth, kq);
                    ViewBag.tbkx = "Danh sách nhân viên "+ kq.ToLower() +" kết quả theo bộ phận: " + tenBoPhan;
                    var users = _context.NguoiDungs
           .Where(nguoiDung => nguoiDung.IdBp == idbp && nguoiDung.ThamGia.Any(thamGia => thamGia.Danhgia == kq));
           
                    return View(await users.ToListAsync());
                }
                if (idbp != 0 && idbp != null)
                {
                    string tenBoPhan = bp.FirstOrDefault(b => b.IdBp == idbp)?.Tenbophan;
                    ViewBag.bophan = new SelectList(bp, "IdBp", "Tenbophan", idbp);
                    ViewBag.tbkx = "Danh sách nhân viên theo bộ phận: " + tenBoPhan;
                    var dtsoftContext2 = _context.NguoiDungs.Include(n => n.IdBpNavigation).Include(n => n.IdVtNavigation).Where(h => h.IdBp == idbp);
                    return View(await dtsoftContext2.ToListAsync());
                }
                if (idct != 0 && idct != null)
                {
                    string tenChitieu = ct.FirstOrDefault(b => b.IdCt == idct)?.Chitieu;
                    ViewBag.chitieu = new SelectList(ct, "IdCt", "Chitieu", idct);
                    ViewBag.tbkx = "Danh sách nhân viên theo chỉ tiêu: " + tenChitieu;
                    var nguoiDungTheoChitieu = _context.NguoiDungs.Where(nguoiDung => nguoiDung.ThamGia.Any(thamGia => thamGia.IdCtNavigation.IdCt == idct));
                    return View(await nguoiDungTheoChitieu.ToListAsync());
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
