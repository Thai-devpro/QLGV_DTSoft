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
    public class VaiTroesController : Controller
    {
        private readonly DtsoftContext _context;

        public VaiTroesController(DtsoftContext context)
        {
            _context = context;
        }

        // GET: VaiTroes
        public async Task<IActionResult> Index()
        {
              return _context.VaiTros != null ? 
                          View(await _context.VaiTros.Include(v => v.CoQuyenTruyCaps).Include(v => v.NguoiDungs).ToListAsync()) :
                          Problem("Entity set 'DtsoftContext.VaiTros'  is null.");
        }
        public async Task<IActionResult> ThemQuyen(int id)
        {
            //if (HttpContext.Session.GetInt32("idtv") == null)
            //{
            //    return RedirectToAction("Login", "ThanhVien");
            //}
            //var count = _context.Quyens.Where(c => c.MaCn == 9 && c.MaCv == HttpContext.Session.GetInt32("cvtv")).Count();
            //if (count == 0)
            //{
            //    return RedirectToAction("norole", "Home");
            //}
            var vaitro = _context.VaiTros.Find(id);
            if (vaitro == null)
            {
                return NotFound();
            }

            var chucnangs = _context.Quyens.Include(c => c.CoQuyenTruyCaps);

            HttpContext.Session.SetInt32("mavt", vaitro.IdVt);
            HttpContext.Session.SetString("tenvt", vaitro.Tenvaitro);

            return View(await chucnangs.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> ThemQuyen(int id, int[] quyen)
        {
            var chucvu = _context.VaiTros.Find(id);
            if (chucvu == null)
            {
                return NotFound();
            }

            // Xóa các quyền cũ
            var quyens = _context.CoQuyenTruyCaps.Where(q => q.IdVt == id).ToList();
            foreach (var q in quyens)
            {
                _context.CoQuyenTruyCaps.Remove(q);
            }

            // Thêm quyền mới
            if (quyen != null)
            {
                foreach (var maCn in quyen)
                {
                    var chucnang = _context.Quyens.Find(maCn);
                    if (chucnang != null)
                    {
                        var q = new CoQuyenTruyCap { IdVt = id, IdQuyen = maCn };
                        _context.CoQuyenTruyCaps.Add(q);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: VaiTroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VaiTros == null)
            {
                return NotFound();
            }

            var vaiTro = await _context.VaiTros
                .FirstOrDefaultAsync(m => m.IdVt == id);
            if (vaiTro == null)
            {
                return NotFound();
            }

            return View(vaiTro);
        }

        // GET: VaiTroes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VaiTroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVt,Tenvaitro,Mota")] VaiTro vaiTro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vaiTro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vaiTro);
        }

        // GET: VaiTroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VaiTros == null)
            {
                return NotFound();
            }

            var vaiTro = await _context.VaiTros.FindAsync(id);
            if (vaiTro == null)
            {
                return NotFound();
            }
            return View(vaiTro);
        }

        // POST: VaiTroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVt,Tenvaitro,Mota")] VaiTro vaiTro)
        {
            if (id != vaiTro.IdVt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vaiTro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VaiTroExists(vaiTro.IdVt))
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
            return View(vaiTro);
        }

        // GET: VaiTroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VaiTros == null)
            {
                return NotFound();
            }

            var vaiTro = await _context.VaiTros
                .FirstOrDefaultAsync(m => m.IdVt == id);
            if (vaiTro == null)
            {
                return NotFound();
            }

            return View(vaiTro);
        }

        // POST: VaiTroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VaiTros == null)
            {
                return Problem("Entity set 'DtsoftContext.VaiTros'  is null.");
            }
            var vaiTro = await _context.VaiTros.FindAsync(id);
            if (vaiTro != null)
            {
                _context.VaiTros.Remove(vaiTro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VaiTroExists(int id)
        {
          return (_context.VaiTros?.Any(e => e.IdVt == id)).GetValueOrDefault();
        }
    }
}
