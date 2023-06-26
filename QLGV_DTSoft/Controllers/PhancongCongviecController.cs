using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using QLGV_DTSoft.Helper;
using QLGV_DTSoft.ViewModel;
using System.Linq;
using System.Security.Claims;

namespace QLGV_DTSoft.Controllers
{
    [CustomAuthorize(5)]
    public class PhancongCongviecController : Controller
    {
        private readonly DtsoftContext _context;

        public PhancongCongviecController(DtsoftContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            /*var count = _context.CoQuyenTruyCaps.Where(c => c.IdQuyen == 5 && c.IdVt == int.Parse(User.FindFirstValue("idvaitro"))).Count();
            if (count == 0)
            {
                return RedirectToAction("norole", "Home");
            }*/

            var bophanIdClaim = User.FindFirstValue("idBophan");
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            var tenBophanClaim = User.FindFirstValue("tenBophan");

            int? bophanId = !string.IsNullOrEmpty(bophanIdClaim) ? int.Parse(bophanIdClaim) : null;
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            string? tenBophan = tenBophanClaim;

            ViewData["tenbophan"] = tenBophan;
            if (bophanId != null && khuvucId != null)
            {
                // Truy vấn danh sách kế hoạch giao việc cho bộ phận của khu vực đó
                var kehoachGiaoViec = await _context.KeHoachGiaoViecs.Include(u => u.IdBpNavigation).ThenInclude(uu => uu.IdKhuvucNavigation).Include(u => u.ThamGia)
                    .Where(kh => kh.IdBpNavigation.IdBp == bophanId && kh.IdBpNavigation.IdKhuvucNavigation.IdKhuvuc == khuvucId  )
                    .ToListAsync();
                return View(kehoachGiaoViec);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddtoPlan(int? id)
        {

            var bophanIdClaim = User.FindFirstValue("idBophan");
            var khuvucIdClaim = User.FindFirstValue("idKhuvuc");
            var nguoidungIdClaim = User.FindFirstValue("idNguoidung");

            int? bophanId = !string.IsNullOrEmpty(bophanIdClaim) ? int.Parse(bophanIdClaim) : null;
            int? khuvucId = !string.IsNullOrEmpty(khuvucIdClaim) ? int.Parse(khuvucIdClaim) : null;
            int? ndId = !string.IsNullOrEmpty(nguoidungIdClaim) ? int.Parse(nguoidungIdClaim) : null;

            var dsNhanvien = await _context.NguoiDungs
                .Include(u => u.IdBpNavigation)
                .ThenInclude(uu => uu.IdKhuvucNavigation)
                .Where(nv => nv.IdBpNavigation.IdBp == bophanId
                            && nv.IdBpNavigation.IdKhuvucNavigation.IdKhuvuc == khuvucId
                            && nv.IdNd != ndId)
                .ToListAsync();

            var keHoachGiaoViec = await _context.KeHoachGiaoViecs.FirstOrDefaultAsync(m => m.IdKh == id);

            var dsChiTieu = await _context.ChiTieus
                        .Where(ct => ct.IdKh == id)
                        .ToListAsync();

            var dsThamGia = await _context.ThamGia
                .Where(tg => tg.IdKh == id)
                .ToListAsync();

            var dsIdThamGia = dsThamGia.Select(tg => tg.IdNd).ToList();

            var viewModel = new AddToPlanViewModel
            {
                KeHoachGiaoViec = keHoachGiaoViec,
                DsNhanvien = dsNhanvien,
                DsIdThamGia = dsIdThamGia,
                ChiTieu = dsChiTieu

            };

            return PartialView("_AddtoPlanPartialView", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSelectedEmployees(List<int> selectedNhanVien, int idKh, int selectedChiTieu)
        {
            var existingThamGia = await _context.ThamGia
                .Where(tg => tg.IdKh == idKh && tg.IdCt == selectedChiTieu)
                .ToListAsync();

            // Tạo một danh sách các ID người dùng đã được chọn
            var selectedNhanVienIds = selectedNhanVien != null ? new HashSet<int>(selectedNhanVien) : new HashSet<int>();

            // Xóa sự tham gia cho những người dùng đã bị bỏ chọn khỏi chỉ tiêu cũ
            foreach (var thamGia in existingThamGia)
            {
                if (!selectedNhanVienIds.Contains(thamGia.IdNd))
                {
                    _context.ThamGia.Remove(thamGia);
                }
            }

            // Thêm sự tham gia mới cho những người dùng đã được chọn và chỉ tiêu mới
            foreach (int nhanVienId in selectedNhanVienIds)
            {
                // Kiểm tra xem sự tham gia đã tồn tại hay chưa
                var existingThamGiaKH = await _context.ThamGia
                    .FirstOrDefaultAsync(tg => tg.IdNd == nhanVienId && tg.IdKh == idKh && tg.IdCt == selectedChiTieu);

                if (existingThamGiaKH == null)
                {
                    ThamGium thamGia = new ThamGium
                    {
                        IdNd = nhanVienId,
                        IdKh = idKh,
                        IdCt = selectedChiTieu
                    };
                    _context.ThamGia.Add(thamGia);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        
        [HttpGet]
        public async Task<IActionResult> EvaluateResult(int id)
        {
            var thamGia = await _context.ThamGia
                .Where(tg => tg.IdKh == id)
                .Include(tg => tg.IdNdNavigation)
                .ToListAsync();
            var chiTieuDs = await _context.ChiTieus
                .Where(ct => ct.IdKh == id)
                .ToListAsync();

            foreach (var thanhVien in thamGia)
            {
                var chiTieu = await _context.ChiTieus
                    .FirstOrDefaultAsync(ct => ct.IdKh == id && ct.IdCt == thanhVien.IdCt);

                if (chiTieu != null)
                {
                    if (thanhVien.SlHoanthanh.HasValue)
                    {
                        double tiLeHoanThanh = ((double)thanhVien.SlHoanthanh.Value / (double)chiTieu.Doanhso) * 100;
                        string danhGia = string.Empty;

                        if (tiLeHoanThanh >= 100)
                            danhGia = "Đạt";
                        else if (tiLeHoanThanh >= 75)
                            danhGia = "Chưa đạt";
                        else
                            danhGia = "Không đạt";

                        thanhVien.Danhgia = danhGia;
                    }
                    else
                    {
                   
                        thanhVien.Danhgia = "Không đạt";
                    }

                    _context.ThamGia.Update(thanhVien);
                }
            }


            await _context.SaveChangesAsync();

            var viewModel = new EvaluateResultViewModel
            {
                ThanhVien = thamGia,
                ChiTieu = chiTieuDs // Không cần gán giá trị cho ChiTieu trong trường hợp này
            };

            return View(viewModel);
        }

    }
}
