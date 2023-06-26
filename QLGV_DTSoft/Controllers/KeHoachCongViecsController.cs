using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using QLGV_DTSoft.Data;

namespace QLGV_DTSoft.Controllers
{
    [Authorize]
    public class KeHoachCongViecsController : Controller
    {
        private readonly DtsoftContext _context;

        public string IdKhcv { get; private set; }

        public KeHoachCongViecsController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: KeHoachCongViecs
        public async Task<IActionResult> Index(string? NamthuchienFormatted)
        {
            var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 3 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }
            var nth = _context.KeHoachCongViecs
                .Select(h => h.NamthuchienFormatted)
                .Distinct()
                .ToList();
            var distinctNth = new List<string>();
            distinctNth.Add("Tất cả");
            foreach (var item in nth)
            {
                if (!distinctNth.Contains(item))
                {
                    distinctNth.Add(item);
                }
            }
            if (!string.IsNullOrEmpty(NamthuchienFormatted) && NamthuchienFormatted != "Tất cả")
            {
                ViewBag.nth = new SelectList(distinctNth, NamthuchienFormatted);
                var khcv = _context.KeHoachCongViecs
                    .AsEnumerable()
                    .Where(h => h.NamthuchienFormatted == NamthuchienFormatted)
                    .ToList();
                return View(khcv);
            }

            ViewBag.nth = new SelectList(distinctNth, NamthuchienFormatted);
            var nth2 = _context.KeHoachCongViecs.ToList();
            return View(nth2);
        }




        // GET: KeHoachCongViecs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KeHoachCongViecs == null)
            {
                return NotFound();
            }

            var keHoachCongViec = await _context.KeHoachCongViecs
                .FirstOrDefaultAsync(m => m.IdKhcv == id);
            if (keHoachCongViec == null)
            {
                return NotFound();
            }

            return View(keHoachCongViec);
        }

        // GET: KeHoachCongViecs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KeHoachCongViecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKhcv,Namthuchien,Noidungcongviec")] KeHoachCongViec keHoachCongViec)
        {
            if (keHoachCongViec.Namthuchien == null)
            {
               
                ModelState.AddModelError("Namthuchien", "Vui lòng chọn năm thực hiện!");
                
                return View(keHoachCongViec);
            }
            if (keHoachCongViec.Noidungcongviec == null)
            {

                ModelState.AddModelError("Noidungcongviec", "Vui lòng nhập nội dung công việc!");

                return View(keHoachCongViec);
            }
            _context.Add(keHoachCongViec);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
          
            return View(keHoachCongViec);
        }

        // GET: KeHoachCongViecs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KeHoachCongViecs == null)
            {
                return NotFound();
            }

            var keHoachCongViec = await _context.KeHoachCongViecs.FindAsync(id);
            if (keHoachCongViec == null)
            {
                return NotFound();
            }
            return View(keHoachCongViec);
        }

        // POST: KeHoachCongViecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKhcv,Namthuchien,Noidungcongviec")] KeHoachCongViec keHoachCongViec)
        {
            if (id != keHoachCongViec.IdKhcv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (keHoachCongViec.Namthuchien == null)
                {

                    ModelState.AddModelError("Namthuchien", "Vui lòng chọn năm thực hiện!");

                    return View(keHoachCongViec);
                }
                if (keHoachCongViec.Noidungcongviec == null)
                {

                    ModelState.AddModelError("Noidungcongviec", "Vui lòng nhập nội dung công việc!");

                    return View(keHoachCongViec);
                }
                try
                {
                    _context.Update(keHoachCongViec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeHoachCongViecExists(keHoachCongViec.IdKhcv))
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
            return View(keHoachCongViec);
        }

        // GET: KeHoachCongViecs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KeHoachCongViecs == null)
            {
                return NotFound();
            }

            var keHoachCongViec = await _context.KeHoachCongViecs
                .FirstOrDefaultAsync(m => m.IdKhcv == id);
            if (keHoachCongViec == null)
            {
                return NotFound();
            }

            return View(keHoachCongViec);
        }

        // POST: KeHoachCongViecs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KeHoachCongViecs == null)
            {
                return Problem("Entity set 'DtsoftContext.KeHoachCongViecs'  is null.");
            }
            var keHoachCongViec = await _context.KeHoachCongViecs.FindAsync(id);
            if (keHoachCongViec != null)
            {
                _context.KeHoachCongViecs.Remove(keHoachCongViec);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeHoachCongViecExists(int id)
        {
          return (_context.KeHoachCongViecs?.Any(e => e.IdKhcv == id)).GetValueOrDefault();
        }
    }
}
