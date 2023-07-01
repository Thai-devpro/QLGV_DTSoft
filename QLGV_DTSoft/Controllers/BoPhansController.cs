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
    [CustomAuthorize(2)]
    public class BoPhansController : Controller
    {
        private readonly DtsoftContext _context;
        private readonly INotyfService _toastNotification;

        public BoPhansController(DtsoftContext context , INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: BoPhans
        public async Task<IActionResult> Index()
        {
            var loggedInUser = UserHelper.GetLoggedInUserKhuvucId(User);
            if (loggedInUser.HasValue)
            {
                int khuvucId = loggedInUser.Value;
                var dtsoftContext = _context.BoPhans.Include(b => b.IdKhuvucNavigation).Where(b => b.IdKhuvucNavigation.IdKhuvuc == khuvucId);
                return View(await dtsoftContext.ToListAsync());
            }
            else
            {
                return View();
            }
        }

        // GET: BoPhans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BoPhans == null)
            {
                return NotFound();
            }

            var boPhan = await _context.BoPhans
                .Include(b => b.IdKhuvucNavigation)
                .FirstOrDefaultAsync(m => m.IdBp == id);
            if (boPhan == null)
            {
                return NotFound();
            }

            return View(boPhan);
        }

        // GET: BoPhans/Create
        public IActionResult Create()
        {

            /*ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "Tenkhuvuc");
            return View();*/
            int? loggedInUserKhuvucId = UserHelper.GetLoggedInUserKhuvucId(User);

            if (loggedInUserKhuvucId.HasValue)
            {
                int khuvucId = loggedInUserKhuvucId.Value;
                var khuvuc = _context.KhuVucs.FirstOrDefault(k => k.IdKhuvuc == khuvucId);

                if (khuvuc != null)
                {
                    ViewData["IdKhuvuc"] = new SelectList(new List<KhuVuc> { khuvuc }, "IdKhuvuc", "Tenkhuvuc");
                    return View();
                }
            }

            // Xử lý khi không tìm thấy ID khu vực hoặc khi không tìm thấy khu vực
            return NotFound();
        }

        // POST: BoPhans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBp,IdKhuvuc,Tenbophan,Congviecchuyenmon")] BoPhan boPhan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boPhan);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Thêm mới bộ phận " + boPhan.Tenbophan + " thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "Tenkhuvuc", boPhan.IdKhuvuc);
            return View(boPhan);
        }

        // GET: BoPhans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BoPhans == null)
            {
                return NotFound();
            }

            var boPhan = await _context.BoPhans.FindAsync(id);
            if (boPhan == null)
            {
                return NotFound();
            }
            int? loggedInUserKhuvucId = UserHelper.GetLoggedInUserKhuvucId(User);

            if (loggedInUserKhuvucId.HasValue)
            {
                int khuvucId = loggedInUserKhuvucId.Value;
                var khuvuc = _context.KhuVucs.FirstOrDefault(k => k.IdKhuvuc == khuvucId);

                if (khuvuc != null)
                {
                    ViewData["IdKhuvuc"] = new SelectList(new List<KhuVuc> { khuvuc }, "IdKhuvuc", "Tenkhuvuc" , boPhan.IdKhuvuc);
                    return View(boPhan);
                }
            }
            return NotFound();
        }

        // POST: BoPhans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBp,IdKhuvuc,Tenbophan,Congviecchuyenmon")] BoPhan boPhan)
        {
            if (id != boPhan.IdBp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boPhan);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Cập nhật " + boPhan.Tenbophan + " thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoPhanExists(boPhan.IdBp))
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
            ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "Tenkhuvuc", boPhan.IdKhuvuc);
            return View(boPhan);
        }

        // GET: BoPhans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BoPhans == null)
            {
                return NotFound();
            }

            var boPhan = await _context.BoPhans
                .Include(b => b.IdKhuvucNavigation)
                .FirstOrDefaultAsync(m => m.IdBp == id);
            if (boPhan == null)
            {
                return NotFound();
            }

            return View(boPhan);
        }

        // POST: BoPhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BoPhans == null)
            {
                return Problem("Entity set 'DtsoftContext.BoPhans'  is null.");
            }
            var boPhan = await _context.BoPhans.FindAsync(id);
            if (boPhan != null)
            {
                _context.BoPhans.Remove(boPhan);
            }
            
            await _context.SaveChangesAsync();
            _toastNotification.Information("Xóa " + boPhan.Tenbophan + " thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool BoPhanExists(int id)
        {
          return (_context.BoPhans?.Any(e => e.IdBp == id)).GetValueOrDefault();
        }
    }
}
