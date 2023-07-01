using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(6)]
    public class KhuVucsController : Controller
    {
        private readonly DtsoftContext _context;
        private readonly INotyfService _toastNotification;

        public KhuVucsController(DtsoftContext context,INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification; 
        }

        // GET: KhuVucs
        public async Task<IActionResult> Index()
        {
           /* var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 6 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }*/
            return _context.KhuVucs != null ? 
                          View(await _context.KhuVucs.ToListAsync()) :
                          Problem("Entity set 'DtsoftContext.KhuVucs'  is null.");
        }

        // GET: KhuVucs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KhuVucs == null)
            {
                return NotFound();
            }

            var khuVuc = await _context.KhuVucs
                .FirstOrDefaultAsync(m => m.IdKhuvuc == id);
            if (khuVuc == null)
            {
                return NotFound();
            }

            return View(khuVuc);
        }

        // GET: KhuVucs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhuVucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKhuvuc,Tenkhuvuc,Diachi,Email,Sodienthoai")] KhuVuc khuVuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khuVuc);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Thêm thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(khuVuc);
        }

        // GET: KhuVucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KhuVucs == null)
            {
                return NotFound();
            }

            var khuVuc = await _context.KhuVucs.FindAsync(id);
            if (khuVuc == null)
            {
                return NotFound();
            }
            return View(khuVuc);
        }

        // POST: KhuVucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKhuvuc,Tenkhuvuc,Diachi,Email,Sodienthoai")] KhuVuc khuVuc)
        {
            if (id != khuVuc.IdKhuvuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khuVuc);
                    await _context.SaveChangesAsync();
                    _toastNotification.Information("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhuVucExists(khuVuc.IdKhuvuc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khuVuc);
        }

        // GET: KhuVucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KhuVucs == null)
            {
                return NotFound();
            }

            var khuVuc = await _context.KhuVucs
                .FirstOrDefaultAsync(m => m.IdKhuvuc == id);
            if (khuVuc == null)
            {
                return NotFound();
            }

            return View(khuVuc);
        }

        // POST: KhuVucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KhuVucs == null)
            {
                return Problem("Entity set 'DtsoftContext.KhuVucs'  is null.");
            }
            var khuVuc = await _context.KhuVucs.FindAsync(id);
            if (khuVuc != null)
            {
                _context.KhuVucs.Remove(khuVuc);
            }
            
            await _context.SaveChangesAsync();
            _toastNotification.Information("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool KhuVucExists(int id)
        {
          return (_context.KhuVucs?.Any(e => e.IdKhuvuc == id)).GetValueOrDefault();
        }
    }
}
