using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;

namespace QLGV_DTSoft.Controllers
{
    public class KeHoachGiaoViecsController : Controller
    {
        private readonly DtsoftContext _context;

        public KeHoachGiaoViecsController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: KeHoachGiaoViecs
        public async Task<IActionResult> Index()
        {
            var dtsoftContext = _context.KeHoachGiaoViecs.Include(k => k.IdBpNavigation).Include(k => k.IdKhcvNavigation);
            return View(await dtsoftContext.ToListAsync());
        }

        // GET: KeHoachGiaoViecs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KeHoachGiaoViecs == null)
            {
                return NotFound();
            }

            var keHoachGiaoViec = await _context.KeHoachGiaoViecs
                .Include(k => k.IdBpNavigation)
                .Include(k => k.IdKhcvNavigation)
                .FirstOrDefaultAsync(m => m.IdKh == id);
            if (keHoachGiaoViec == null)
            {
                return NotFound();
            }

            return View(keHoachGiaoViec);
        }

        // GET: KeHoachGiaoViecs/Create
        public IActionResult Create()
        {
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan");
            ViewData["IdKhcv"] = new SelectList(_context.KeHoachCongViecs, "IdKhcv", "NamthuchienFormatted");
            return View();
        }

        // POST: KeHoachGiaoViecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKh,IdBp,IdKhcv,Tenkehoach,Ngaybatdau,Ngayketthuc,Motakh,Ngaytaokh")] KeHoachGiaoViec keHoachGiaoViec)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keHoachGiaoViec);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
            ViewData["IdKhcv"] = new SelectList(_context.KeHoachCongViecs, "IdKhcv", "NamthuchienFormatted", keHoachGiaoViec.IdKhcv);
            return View(keHoachGiaoViec);
        }

        // GET: KeHoachGiaoViecs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KeHoachGiaoViecs == null)
            {
                return NotFound();
            }

            var keHoachGiaoViec = await _context.KeHoachGiaoViecs.FindAsync(id);
            if (keHoachGiaoViec == null)
            {
                return NotFound();
            }
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
            ViewData["IdKhcv"] = new SelectList(_context.KeHoachCongViecs, "IdKhcv", "NamthuchienFormatted", keHoachGiaoViec.IdKhcv);
            return View(keHoachGiaoViec);
        }

        // POST: KeHoachGiaoViecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKh,IdBp,IdKhcv,Tenkehoach,Ngaybatdau,Ngayketthuc,Motakh,Ngaytaokh")] KeHoachGiaoViec keHoachGiaoViec)
        {
            if (id != keHoachGiaoViec.IdKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keHoachGiaoViec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeHoachGiaoViecExists(keHoachGiaoViec.IdKh))
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
            ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
            ViewData["IdKhcv"] = new SelectList(_context.KeHoachCongViecs, "IdKhcv", "NamthuchienFormatted", keHoachGiaoViec.IdKhcv);
            return View(keHoachGiaoViec);
        }

        // GET: KeHoachGiaoViecs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KeHoachGiaoViecs == null)
            {
                return NotFound();
            }

            var keHoachGiaoViec = await _context.KeHoachGiaoViecs
                .Include(k => k.IdBpNavigation)
                .Include(k => k.IdKhcvNavigation)
                .FirstOrDefaultAsync(m => m.IdKh == id);
            if (keHoachGiaoViec == null)
            {
                return NotFound();
            }

            return View(keHoachGiaoViec);
        }

        // POST: KeHoachGiaoViecs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KeHoachGiaoViecs == null)
            {
                return Problem("Entity set 'DtsoftContext.KeHoachGiaoViecs'  is null.");
            }
            var keHoachGiaoViec = await _context.KeHoachGiaoViecs.FindAsync(id);
            if (keHoachGiaoViec != null)
            {
                _context.KeHoachGiaoViecs.Remove(keHoachGiaoViec);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeHoachGiaoViecExists(int id)
        {
          return (_context.KeHoachGiaoViecs?.Any(e => e.IdKh == id)).GetValueOrDefault();
        }
    }
}
