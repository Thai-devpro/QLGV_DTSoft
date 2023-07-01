using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;

namespace QLGV_DTSoft.Controllers
{
    [Authorize]
    public class UserprofileController : Controller
    {
        private readonly DtsoftContext _context;
        private readonly INotyfService _toastNotification;
        public UserprofileController(DtsoftContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null || _context.NguoiDungs == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
            ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
            return View(nguoiDung);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("IdNd,IdVt,IdBp,Tennguoidung,Matkhau,Hoten,Ngaysinh,Gioitinh,Sodienthoai,Diachi,Email,Quequan,Ngaybatdaulam,Thamnien")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.IdNd)
            {
                return NotFound();
            }
            try
            {
                var nguoiDung2 = _context.NguoiDungs.AsNoTracking().SingleOrDefault(n => n.IdNd == id);
                nguoiDung.IdVt = nguoiDung2.IdVt;
                nguoiDung.IdBp = nguoiDung2.IdBp;

                nguoiDung.Ngaybatdaulam = nguoiDung2.Ngaybatdaulam;
                nguoiDung.Thamnien = nguoiDung2.Thamnien;

                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();
                _toastNotification.Information("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NguoiDungExists(nguoiDung.IdNd))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
            ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
           
            return View(nguoiDung);

            //ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
            //ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
            //return View(nguoiDung);
        }

        public async Task<IActionResult> EditMK(int? id)
        {
            if (id == null || _context.NguoiDungs == null)
            {
                return NotFound();
            }


            var nguoiDung = _context.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMK(int id, NguoiDung nguoiDung2, string Matkhaumoi, string Matkhaulap)
        {
            var nguoiDung = _context.NguoiDungs.AsNoTracking().SingleOrDefault(n => n.IdNd == id);
            if (Matkhaumoi == null)
            {

                ViewBag.matkhaumoi = "Nhập mật khẩu mới!";
                return View(nguoiDung);
            }
            if (Matkhaumoi == null)
            {

                ViewBag.matkhaulap = "Lập lại mật khẩu";
                return View(nguoiDung);
            }
            if (Matkhaumoi != Matkhaulap)
            {

                ViewBag.matkhaulap = "Mật khẩu không khớp";
                return View(nguoiDung);
            }

            if (nguoiDung == null)
            {
                return NotFound();
            }
            try
            {
                nguoiDung.Matkhau = SecretHasher.Hash(Matkhaumoi);
                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();

                
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            ViewBag.tbmk = "Cập nhật mật khâu thành công";
            return View(nguoiDung);

            //ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
            //ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
            //return View(nguoiDung);
        }

        private bool NguoiDungExists(int id)
        {
            return (_context.NguoiDungs?.Any(e => e.IdNd == id)).GetValueOrDefault();
        }
    }
}
