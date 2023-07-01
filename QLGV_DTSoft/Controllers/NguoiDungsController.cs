using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;
using MailKit.Net.Smtp;
using MailKit;
using System.Net;
using MimeKit;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(1)]
    public class NguoiDungsController : Controller
    {
        private readonly DtsoftContext _context;
        private readonly INotyfService _toastNotification;
        public NguoiDungsController(DtsoftContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: NguoiDungs
        public async Task<IActionResult> Index()
        {
       
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            var dtsoftContext = _context.NguoiDungs.Include(n => n.IdBpNavigation).ThenInclude(kv => kv.IdKhuvucNavigation).Include(n => n.IdVtNavigation).Where(n => n.IdBpNavigation.IdKhuvuc == khuvucId);
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
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(b => b.IdKhuvuc == khuvucId), "IdBp", "Tenbophan");
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
                var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
                int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
                ViewData["IdBp"] = new SelectList(_context.BoPhans.Where(b => b.IdKhuvuc == khuvucId), "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Tennguoidung", "Nhập tên người dùng!");
                return View(nguoiDung);
            }
            var existingNguoiDungWithSameTennguoidung = _context.NguoiDungs.FirstOrDefault(n => n.Tennguoidung == nguoiDung.Tennguoidung);
            if (existingNguoiDungWithSameTennguoidung != null)
            {
                ModelState.AddModelError("Tennguoidung", "Tên người dùng đã tồn tại!.");
                return View(nguoiDung);
            }
            
            if (nguoiDung.Hoten == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Hoten", "Nhập họ tên!");
                return View(nguoiDung);
            }
            if (nguoiDung.Ngaysinh == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Ngaysinh", "Chọn ngày sinh!");
                return View(nguoiDung);
            }
            string regex = @"^([\+]?33[-]?|[0])?[1-9][0-9]{8}$";
            if (nguoiDung.Sodienthoai == null || !Regex.IsMatch(nguoiDung.Sodienthoai.ToString(), regex))
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Sodienthoai", "Nhập số điện thoại chính xác!");
                return View(nguoiDung);
            }
            if (nguoiDung.Diachi == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Diachi", "Nhập địa chỉ hiện tại!");
                return View(nguoiDung);
            }
            if (nguoiDung.Quequan == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Quequan", "Nhập quê quán!");
                return View(nguoiDung);
            }
            if (nguoiDung.Ngaybatdaulam == null)
            {
                nguoiDung.Ngaybatdaulam = DateTime.Now;
            }
            string regex2 = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            if (nguoiDung.Email == null || !Regex.IsMatch(nguoiDung.Email.ToString().Trim(), regex2))
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Email", "Vui lòng nhập email chính xác!");
                return View(nguoiDung);
            }
            var password = GenerateRandomPassword();
            nguoiDung.Matkhau = SecretHasher.Hash(password);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DTSoft", "DTSoft@gmail.com"));
            message.To.Add(new MailboxAddress("Thành Viên", nguoiDung.Email));
            message.Subject = "Thông tin tài khoản người dùng";
            message.Body = new TextPart("plain")
            {
                Text = $"Gửi {nguoiDung.Hoten},\n\nTài khoản của bạn đã được tạo.\nTên đăng nhập: {nguoiDung.Tennguoidung}.\nMật khẩu: {password}"
            };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com");
            client.Authenticate("devthai3401@gmail.com", "mfpcaknsbfmwrvzd");
            client.Send(message);
            client.Disconnect(true);


            _context.Add(nguoiDung);
            await _context.SaveChangesAsync();
            _toastNotification.Success("Thêm thành công");
            return RedirectToAction(nameof(Index));
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // GET: NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NguoiDungs == null)
            {
                return NotFound();
            }

            var nguoiDung =  _context.NguoiDungs.AsNoTracking().FirstOrDefault(n => n.IdNd == id);
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
            if (nguoiDung.Tennguoidung == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Tennguoidung", "Nhập tên người dùng!");
                return View(nguoiDung);
            }
            var existingNguoiDungWithSameTennguoidung = _context.NguoiDungs.AsNoTracking().FirstOrDefault(n => n.Tennguoidung == nguoiDung.Tennguoidung);
            if (existingNguoiDungWithSameTennguoidung != null && existingNguoiDungWithSameTennguoidung.IdNd != nguoiDung.IdNd)
            {
                ModelState.AddModelError("Tennguoidung", "Tên người dùng đã tồn tại!.");
                return View(nguoiDung);
            }

            if (nguoiDung.Hoten == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Hoten", "Nhập họ tên!");
                return View(nguoiDung);
            }
            if (nguoiDung.Ngaysinh == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Ngaysinh", "Chọn ngày sinh!");
                return View(nguoiDung);
            }
            string regex = @"^([\+]?33[-]?|[0])?[1-9][0-9]{8}$";
            if (nguoiDung.Sodienthoai == null || !Regex.IsMatch(nguoiDung.Sodienthoai.ToString(), regex))
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Sodienthoai", "Nhập số điện thoại chính xác!");
                return View(nguoiDung);
            }
            if (nguoiDung.Diachi == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Diachi", "Nhập địa chỉ hiện tại!");
                return View(nguoiDung);
            }
            if (nguoiDung.Quequan == null)
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Quequan", "Nhập quê quán!");
                return View(nguoiDung);
            }
            if (nguoiDung.Ngaybatdaulam == null)
            {
                nguoiDung.Ngaybatdaulam = DateTime.Now;
            }
            string regex2 = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            if (nguoiDung.Email == null || !Regex.IsMatch(nguoiDung.Email.ToString().Trim(), regex2))
            {
                ViewData["IdBp"] = new SelectList(_context.BoPhans, "IdBp", "Tenbophan", nguoiDung.IdBp);
                ViewData["IdVt"] = new SelectList(_context.VaiTros, "IdVt", "Tenvaitro", nguoiDung.IdVt);
                ModelState.AddModelError("Email", "Vui lòng nhập email chính xác!");
                return View(nguoiDung);
            }

            try
            {
                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Cập nhật thành công");
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
            _toastNotification.Information("Đã xóa " + nguoiDung.Hoten);
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
          return (_context.NguoiDungs?.Any(e => e.IdNd == id)).GetValueOrDefault();
        }
    }
}
