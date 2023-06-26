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
            
            var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 1 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }

            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            var dtsoftContext = _context.NguoiDungs.Include(n => n.IdBpNavigation).ThenInclude(kv => kv.IdKhuvucNavigation).Include(n => n.IdVtNavigation).Where(n => n.IdBpNavigation.IdKhuvuc == khuvucId); ;
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
