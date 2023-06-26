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

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(2)]
    public class BoPhansController : Controller
    {
        private readonly DtsoftContext _context;

        public BoPhansController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: BoPhans
        public async Task<IActionResult> Index()
        {
            /*var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 2 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }*/

            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            var dtsoftContext = _context.BoPhans.Include(b => b.IdKhuvucNavigation).Where(b => b.IdKhuvucNavigation.IdKhuvuc == khuvucId);

            return View(await dtsoftContext.ToListAsync());
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
            ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "Tenkhuvuc");
            return View();
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
            ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "IdKhuvuc", boPhan.IdKhuvuc);
            return View(boPhan);
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
            ViewData["IdKhuvuc"] = new SelectList(_context.KhuVucs, "IdKhuvuc", "IdKhuvuc", boPhan.IdKhuvuc);
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
            return RedirectToAction(nameof(Index));
        }

        private bool BoPhanExists(int id)
        {
          return (_context.BoPhans?.Any(e => e.IdBp == id)).GetValueOrDefault();
        }
    }
}
