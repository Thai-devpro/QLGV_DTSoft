﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;
using SautinSoft.Document;
using SautinSoft.Document.Drawing;

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

            DocumentCore dc = new DocumentCore();
            DocumentBuilder db = new DocumentBuilder(dc);
         // Thêm văn bản trước khi tải HTML
            db.CharacterFormat.FontName = "Verdana";
            db.CharacterFormat.Size = 16f;
            db.CharacterFormat.AllCaps = true;
            db.CharacterFormat.Italic = true;
            db.CharacterFormat.FontColor = Color.Orange;
            db.Write("Công ty phần mềm DTSoft");
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Blue;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.Write("Phần mềm hiệu quả");
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Black;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.ParagraphFormat.Alignment = HorizontalAlignment.Center;
            db.Write("Báo cáo");

            // Tải HTML vào tài liệu
            db.InsertHtml(GridHtml);

            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Black;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.Write("Ngày lập báo cáo: "+ DateTime.Now);
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.Write("Người lập: " + User.Identity.Name);
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = true;
            db.Write("(Ký và ghi rõ họ tên)");
           

            // Lưu tài liệu thành file DOCX
            dc.Save(output);
            byte[] bytes = System.IO.File.ReadAllBytes(output);

            // Xóa thư mục tạm
            Directory.Delete(path, true);

            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Danhsachnguoidung.docx");
        }
        [HttpPost]
        public IActionResult ExportToPdf(string GridHtml)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "HTML");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string input = Path.Combine(path, "html1.html");
            string output = Path.Combine(path, "Danhsachnguoidung.pdf");
            System.IO.File.WriteAllText(input, GridHtml);

            DocumentCore dc = new DocumentCore();
            DocumentBuilder db = new DocumentBuilder(dc);

            // Thêm văn bản trước khi tải HTML
            db.CharacterFormat.FontName = "Verdana";
            db.CharacterFormat.Size = 16f;
            db.CharacterFormat.AllCaps = true;
            db.CharacterFormat.Italic = true;
            db.CharacterFormat.FontColor = Color.Orange;
            db.Write("Công ty phần mềm DTSoft");
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Blue;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.Write("Phần mềm hiệu quả");
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Black;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.ParagraphFormat.Alignment = HorizontalAlignment.Center;
            db.Write("Báo cáo");

            // Tải HTML vào tài liệu
            db.InsertHtml(GridHtml);

            db.CharacterFormat.Size = 13f;
            db.CharacterFormat.FontColor = Color.Black;
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = false;
            db.Write("Ngày lập báo cáo: " + DateTime.Now);
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.Write("Người lập: " + User.Identity.Name);
            db.InsertSpecialCharacter(SpecialCharacterType.LineBreak);
            db.CharacterFormat.AllCaps = false;
            db.CharacterFormat.Italic = true;
            db.Write("(Ký và ghi rõ họ tên)");

            // Lưu tài liệu thành file PDF
            dc.Save(output, new PdfSaveOptions());
            byte[] bytes = System.IO.File.ReadAllBytes(output);

            // Xóa thư mục tạm
            Directory.Delete(path, true);

            return File(bytes, "application/pdf", "Danhsachnguoidung.pdf");
        }



        public async Task<IActionResult> Index(int? idbp, int? idct, string? kq)
        {
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
            ViewBag.tbkx = "Danh sách nhân viên";
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
