﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(4)]
    public class KeHoachGiaoViecsController : Controller
    {
        private readonly DtsoftContext _context;
        private readonly INotyfService _toastNotification;

        public KeHoachGiaoViecsController(DtsoftContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: KeHoachGiaoViecs
        public async Task<IActionResult> Index()
        {
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;

            var dtsoftContext = _context.KeHoachGiaoViecs.Include(k => k.IdBpNavigation).ThenInclude(kv => kv.IdKhuvucNavigation).Include(k => k.IdKhcvNavigation)
                .Where(k => k.IdBpNavigation.IdKhuvuc == khuvucId);
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
                .Include(k => k.ChiTieus)
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
            int? loggedInUserKhuvucId = UserHelper.GetLoggedInUserKhuvucId(User);

            if (loggedInUserKhuvucId.HasValue)
            {
                int khuvucId = loggedInUserKhuvucId.Value;
                var khuvuc = _context.KhuVucs.FirstOrDefault(k => k.IdKhuvuc == khuvucId);

                if (khuvuc != null)
                {
                    ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(n => n.IdKhuvuc == khuvucId), "IdBp", "Tenbophan");
                    var keHoachCongViecs = _context.KeHoachCongViecs.ToList();
                    var selectListItems = keHoachCongViecs.Select(khcv => new SelectListItem
                    {
                        Value = khcv.IdKhcv.ToString(),
                        Text = $"{khcv.NamthuchienFormatted} - {khcv.Noidungcongviec}"
                    });

                    ViewData["IdKhcv"] = new SelectList(selectListItems, "Value", "Text");
                    return View();
                }
            }
            return NotFound();
        }

        // POST: KeHoachGiaoViecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKh,IdBp,IdKhcv,Tenkehoach,Ngaybatdau,Ngayketthuc,Motakh,Ngaytaokh")] KeHoachGiaoViec keHoachGiaoViec, string chitieuList)
        {
            int? loggedInUserKhuvucId = UserHelper.GetLoggedInUserKhuvucId(User);

            if (loggedInUserKhuvucId.HasValue)
            {
                int khuvucId = loggedInUserKhuvucId.Value;
                var khuvuc = _context.KhuVucs.FirstOrDefault(k => k.IdKhuvuc == khuvucId);

                if (khuvuc != null)
                {
                    if (ModelState.IsValid)
                    {
                        // Deserialize danh sách chỉ tiêu từ chuỗi JSON
                        List<ChiTieu> chiTieus = JsonConvert.DeserializeObject<List<ChiTieu>>(chitieuList);

                        // Gán danh sách chỉ tiêu cho kế hoạch giao việc
                        keHoachGiaoViec.ChiTieus = chiTieus;

                        // Lưu kế hoạch giao việc và danh sách chỉ tiêu vào database
                        _context.Add(keHoachGiaoViec);
                        await _context.SaveChangesAsync();
                        _toastNotification.Success("Thêm kế hoạch giao việc thành công");

                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(kv => kv.IdKhuvuc == khuvucId), "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
                    var keHoachCongViecs = _context.KeHoachCongViecs.ToList();
                    var selectListItems = keHoachCongViecs.Select(khcv => new SelectListItem
                    {
                        Value = khcv.IdKhcv.ToString(),
                        Text = $"{khcv.NamthuchienFormatted} - {khcv.Noidungcongviec}",
                        Selected = khcv.IdKhcv == keHoachGiaoViec.IdKhcv
                    });

                    ViewData["IdKhcv"] = new SelectList(selectListItems, "Value", "Text");

                    return View(keHoachGiaoViec);
                }
            }
            return NotFound();
        }


        // GET: KeHoachGiaoViecs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KeHoachGiaoViecs == null)
            {
                return NotFound();
            }
            var keHoachGiaoViec = await _context.KeHoachGiaoViecs
                
                .Include(k => k.ChiTieus)
                .FirstOrDefaultAsync(m => m.IdKh == id);
            if (keHoachGiaoViec == null)
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
                    ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(kv => kv.IdKhuvuc == khuvucId), "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
                   
                    var keHoachCongViecs = _context.KeHoachCongViecs.ToList();
                    var selectListItems = keHoachCongViecs.Select(khcv => new SelectListItem
                    {
                        Value = khcv.IdKhcv.ToString(),
                        Text = $"{khcv.NamthuchienFormatted} - {khcv.Noidungcongviec}",
                        Selected = khcv.IdKhcv == keHoachGiaoViec.IdKhcv
                    });

                    ViewData["IdKhcv"] = new SelectList(selectListItems, "Value", "Text");
                    return View(keHoachGiaoViec);
                }
            }
            return NotFound();
        }

        // POST: KeHoachGiaoViecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKh,IdBp,IdKhcv,Tenkehoach,Ngaybatdau,Ngayketthuc,Motakh,Ngaytaokh")] KeHoachGiaoViec keHoachGiaoViec, string chitieuList)
        {
            if (id != keHoachGiaoViec.IdKh)
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
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            // Deserialize danh sách chỉ tiêu từ chuỗi JSON
                            List<ChiTieu> chiTieus = JsonConvert.DeserializeObject<List<ChiTieu>>(chitieuList);

                            // Lấy danh sách các chỉ tiêu hiện có trong kế hoạch giao việc
                            List<ChiTieu> existingChiTieus = _context.ChiTieus.Where(c => c.IdKh == keHoachGiaoViec.IdKh).ToList();

                            // Cập nhật thông tin các chỉ tiêu
                            foreach (var chitieu in chiTieus)
                            {
                                // Kiểm tra xem chỉ tiêu đã tồn tại trong danh sách hiện có hay chưa
                                var existingChitieu = existingChiTieus.FirstOrDefault(c => c.IdCt == chitieu.IdCt);

                                if (existingChitieu != null)
                                {
                                    // Cập nhật các thuộc tính của chỉ tiêu đã tồn tại
                                    existingChitieu.Chitieu = chitieu.Chitieu;
                                    existingChitieu.Doanhso = chitieu.Doanhso;
                                    existingChitieu.Donvitinh = chitieu.Donvitinh;

                                    _context.Update(existingChitieu);
                                }

                            }

                            // Cập nhật thông tin kế hoạch giao việc
                            _context.Update(keHoachGiaoViec);
                            await _context.SaveChangesAsync();
                            _toastNotification.Information("Cập nhật thành công");

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
                    var keHoachGiaoViecEdit = await _context.KeHoachGiaoViecs.Include(k => k.ChiTieus).FirstOrDefaultAsync(m => m.IdKh == id);
                    ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(kv => khuvuc.IdKhuvuc == khuvucId), "IdBp", "Tenbophan", keHoachGiaoViec.IdBp);
                    var keHoachCongViecs = _context.KeHoachCongViecs.ToList();
                    var selectListItems = keHoachCongViecs.Select(khcv => new SelectListItem
                    {
                        Value = khcv.IdKhcv.ToString(),
                        Text = $"{khcv.NamthuchienFormatted} - {khcv.Noidungcongviec}",
                        Selected = khcv.IdKhcv == keHoachGiaoViec.IdKhcv
                    });
                    ViewData["IdKhcv"] = new SelectList(selectListItems, "Value", "Text");
                    return View(keHoachGiaoViecEdit);
                }
            }
            return NotFound();
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
                var chiTieus = _context.ChiTieus.Where(ct => ct.IdKh == id).ToList();
                if (chiTieus.Count > 0)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng xóa các chỉ tiêu liên quan trước khi xóa kế hoạch giao việc.");
                    return View(keHoachGiaoViec);
                }
                _context.KeHoachGiaoViecs.Remove(keHoachGiaoViec);
            }
            
            await _context.SaveChangesAsync();
            _toastNotification.Information("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool KeHoachGiaoViecExists(int id)
        {
          return (_context.KeHoachGiaoViecs?.Any(e => e.IdKh == id)).GetValueOrDefault();
        }

        [HttpPost]
        public IActionResult DeleteChiTieu(int chitieuId)
        {
            try
            {
              
                var chitieu = _context.ChiTieus.Find(chitieuId);

                if (chitieu == null)
                {
               
                    return Json(new { success = false, error = "Chỉ tiêu không tồn tại." });
                }

            
                _context.ChiTieus.Remove(chitieu);
                _context.SaveChanges();
                _toastNotification.Information("Xóa chỉ tiêu " + chitieu.Chitieu + " thành công");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddChitieu([FromBody] ChiTieu chitieuListNew)
        {
            if (chitieuListNew != null)
            {
                // Tạo một đối tượng ChiTieu từ ChiTieuViewModel
                var chiTieu = new ChiTieu
                {
                    Chitieu = chitieuListNew.Chitieu,
                    Doanhso = chitieuListNew.Doanhso,
                    Donvitinh = chitieuListNew.Donvitinh,
                    IdKh = chitieuListNew.IdKh
                };

                _context.Add(chiTieu);
                await _context.SaveChangesAsync();
                _toastNotification.Information("Thêm chỉ tiêu thành công");
                return Ok();
            }
            return BadRequest("Danh sách chỉ tiêu mới trống");
        }

    }
}
