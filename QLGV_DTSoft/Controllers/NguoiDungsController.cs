using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;

namespace QLGV_DTSoft.Controllers
{
    [Authorize]
    public class NguoiDungsController : Controller
    {
        private readonly DtsoftContext _context;

        public NguoiDungsController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: NguoiDungs
        public async Task<IActionResult> Index()
        {
            var dtsoftContext = _context.NguoiDungs.Include(n => n.IdBpNavigation).Include(n => n.IdVtNavigation);
            return View(await dtsoftContext.ToListAsync());
        }

        // GET: NguoiDungs/Details/5
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

        // GET: NguoiDungs/Create
        public IActionResult Create()
        {
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan");
            ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro");
            return View();
        }

        // POST: NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNd,IdVt,IdBp,Tennguoidung,Matkhau,Hoten,Ngaysinh,Gioitinh,Sodienthoai,Diachi,Email,Quequan,Ngaybatdaulam,Thamnien")] NguoiDung nguoiDung)
        {
            if(nguoiDung.Tennguoidung == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Tennguoidung", "Nhập tên người dùng!");
                return View(nguoiDung);
            }

            if (nguoiDung.Matkhau == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Matkhau", "Nhập mật khẩu!");
                return View(nguoiDung);
            }
            nguoiDung.Matkhau = SecretHasher.Hash(nguoiDung.Matkhau);
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            
        }

        // GET: NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNd,IdVt,IdBp,Tennguoidung,Matkhau,Hoten,Ngaysinh,Gioitinh,Sodienthoai,Diachi,Email,Quequan,Ngaybatdaulam,Thamnien")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.IdNd)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
           
            //ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
            //ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
            //return View(nguoiDung);
        }
        public async Task<IActionResult> Edit2(int? id)
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

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(int id, [Bind("IdNd,IdVt,IdBp,Tennguoidung,Matkhau,Hoten,Ngaysinh,Gioitinh,Sodienthoai,Diachi,Email,Quequan,Ngaybatdaulam,Thamnien")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.IdNd)
            {
                return NotFound();
            }


            try
            {
                var nguoiDung2 =  _context.NguoiDungs.AsNoTracking().SingleOrDefault(n => n.IdNd == id);
                nguoiDung.IdVt = nguoiDung2.IdVt;
                nguoiDung.IdBp = nguoiDung2.IdBp;

                nguoiDung.Ngaybatdaulam = nguoiDung2.Ngaybatdaulam;
                nguoiDung.Thamnien = nguoiDung2.Thamnien;

                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();
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
            ViewBag.tb = "Chỉnh sửa thành công";
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

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMK(int id, NguoiDung nguoiDung2, string Matkhaumoi, string Matkhaulap)
        {
            var nguoiDung = _context.NguoiDungs.AsNoTracking().SingleOrDefault(n => n.IdNd == id);
            if (Matkhaumoi == null )
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
        // GET: NguoiDungs/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: NguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.NguoiDungs == null)
            {
                return Problem("Entity set 'DtsoftContext.NguoiDungs'  is null.");
            }
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
          return (_context.NguoiDungs?.Any(e => e.IdNd == id)).GetValueOrDefault();
        }
    }
}
