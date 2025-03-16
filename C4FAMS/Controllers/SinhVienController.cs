using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class SinhVienController : Controller
    {
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly IChuyenNganhRepository _chuyenNganhRepository;
        private readonly IKhoaRepository _khoaRepository;
        private readonly UserManager<NguoiDung> _userManager;

        public SinhVienController(ISinhVienRepository sinhVienRepository, IChuyenNganhRepository chuyenNganhRepository, IKhoaRepository khoaRepository, UserManager<NguoiDung> userManager)
        {
            _sinhVienRepository = sinhVienRepository;
            _chuyenNganhRepository = chuyenNganhRepository;
            _khoaRepository = khoaRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? khoaId, int? chuyenNganhId, string khoaHoc)
        {
            IEnumerable<SinhVien> sinhVienList;
            if (User.IsInRole("Admin"))
            {
                // Lấy danh sách khoa cho Admin
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;

                // Lấy danh sách chuyên ngành dựa trên khoa được chọn
                if (khoaId.HasValue)
                {
                    ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(khoaId.Value);
                }
                else
                {
                    ViewBag.ChuyenNganhList = new List<ChuyenNganh>();
                }

                // Lấy tất cả sinh viên
                sinhVienList = await _sinhVienRepository.GetAllAsync();
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue) return Forbid("Bạn không thuộc khoa nào.");

                khoaId = user.MaKhoa.Value;
                ViewBag.KhoaId = khoaId;
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(khoaId.Value);

                // Lấy sinh viên thuộc khoa của người dùng
                sinhVienList = await _sinhVienRepository.GetByKhoaAsync(khoaId.Value);
            }
            else
            {
                return Forbid();
            }

            // Lấy danh sách khóa học từ dữ liệu sinh viên
            var khoaHocList = sinhVienList.Select(s => s.KhoaHoc).Distinct().OrderBy(k => k).ToList();
            ViewBag.KhoaHocList = khoaHocList;

            // Áp dụng bộ lọc
            if (khoaId.HasValue)
            {
                sinhVienList = sinhVienList.Where(s => s.ChuyenNganh?.MaKhoa == khoaId.Value);
            }

            if (chuyenNganhId.HasValue)
            {
                sinhVienList = sinhVienList.Where(s => s.MaChuyenNganh == chuyenNganhId.Value);
            }

            if (!string.IsNullOrEmpty(khoaHoc))
            {
                sinhVienList = sinhVienList.Where(s => s.KhoaHoc == khoaHoc);
            }

            // Truyền các giá trị đã lọc vào ViewBag để giữ trạng thái dropdown
            ViewBag.SelectedKhoaId = khoaId;
            ViewBag.SelectedChuyenNganhId = chuyenNganhId;
            ViewBag.SelectedKhoaHoc = khoaHoc;

            return View(sinhVienList);
        }

        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                if (khoaList == null || !khoaList.Any())
                {
                    return RedirectToAction("Index", "Khoa", new { message = "Vui lòng thêm khoa trước khi thêm sinh viên." });
                }
                ViewBag.KhoaList = khoaList;
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(khoaList.First().MaKhoa);
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue) return Forbid("Bạn không thuộc khoa nào.");
                ViewBag.KhoaId = user.MaKhoa.Value;
                var chuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(user.MaKhoa.Value);
                Console.WriteLine($"ChuyenNganhList count: {(chuyenNganhList != null ? chuyenNganhList.Count() : 0)}");
                if (chuyenNganhList == null || !chuyenNganhList.Any())
                {
                    return RedirectToAction("Index", "ChuyenNganh", new { message = "Vui lòng thêm chuyên ngành cho khoa trước khi thêm sinh viên." });
                }
                ViewBag.ChuyenNganhList = chuyenNganhList;
            }
            return View(new SinhVien());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien, int? khoaId)
        {
            // Xử lý ngày sinh
            if (Request.Form["NgaySinh"].Count > 0)
            {
                var ngaySinhStr = Request.Form["NgaySinh"].ToString();
                if (DateTime.TryParseExact(ngaySinhStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngaySinh))
                {
                    if (ngaySinh < new DateTime(1900, 1, 1))
                    {
                        ModelState.AddModelError("NgaySinh", "Ngày sinh phải từ 01/01/1900 trở đi.");
                    }
                    else
                    {
                        sinhVien.NgaySinh = ngaySinh;
                    }
                }
                else
                {
                    ModelState.AddModelError("NgaySinh", "Định dạng ngày sinh không hợp lệ.");
                }
            }
            else
            {
                ModelState.AddModelError("NgaySinh", "Vui lòng chọn ngày sinh.");
            }

            // Kiểm tra MaChuyenNganh
            if (sinhVien.MaChuyenNganh == 0)
            {
                ModelState.AddModelError("MaChuyenNganh", "Vui lòng chọn chuyên ngành.");
            }
            else
            {
                // Gán ChuyenNganh để tránh lỗi validation
                sinhVien.ChuyenNganh = await _chuyenNganhRepository.GetByIdAsync(sinhVien.MaChuyenNganh);
                if (sinhVien.ChuyenNganh == null)
                {
                    ModelState.AddModelError("MaChuyenNganh", "Chuyên ngành không tồn tại.");
                }
                else if (User.IsInRole("Khoa"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (!user.MaKhoa.HasValue) return Forbid();
                    if (sinhVien.ChuyenNganh.MaKhoa != user.MaKhoa.Value)
                    {
                        ModelState.AddModelError("MaChuyenNganh", "Chuyên ngành không thuộc khoa của bạn.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Debug log
                    var chuyenNganhTen = sinhVien.ChuyenNganh?.TenChuyenNganh;
                    Console.WriteLine($"Thêm sinh viên: MSSV={sinhVien.MSSV}, MaChuyenNganh={sinhVien.MaChuyenNganh}, TenChuyenNganh={chuyenNganhTen}");
                    await _sinhVienRepository.AddAsync(sinhVien);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm sinh viên: {ex.Message}");
                }
            }

            // Gán lại ViewBag khi ModelState không hợp lệ
            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;
                ViewBag.ChuyenNganhList = khoaId.HasValue
                    ? await _chuyenNganhRepository.GetByKhoaAsync(khoaId.Value)
                    : await _chuyenNganhRepository.GetByKhoaAsync(khoaList.First().MaKhoa);
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.KhoaId = user.MaKhoa.Value;
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(user.MaKhoa.Value);
            }
            return View(sinhVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || sinhVien.ChuyenNganh?.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể sửa sinh viên thuộc khoa của mình.");
                }
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(sinhVien.ChuyenNganh.MaKhoa);
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.KhoaId = user.MaKhoa.Value;
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(user.MaKhoa.Value);
            }
            return View(sinhVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SinhVien sinhVien, int? khoaId)
        {
            if (id != sinhVien.MSSV) return NotFound();

            if (Request.Form["NgaySinh"].Count > 0)
            {
                var ngaySinhStr = Request.Form["NgaySinh"].ToString();
                if (DateTime.TryParseExact(ngaySinhStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ngaySinh))
                {
                    if (ngaySinh < new DateTime(1900, 1, 1))
                    {
                        ModelState.AddModelError("NgaySinh", "Ngày sinh phải từ 01/01/1900 trở đi.");
                    }
                    else
                    {
                        sinhVien.NgaySinh = ngaySinh;
                    }
                }
                else
                {
                    ModelState.AddModelError("NgaySinh", "Định dạng ngày sinh không hợp lệ.");
                }
            }

            if (sinhVien.MaChuyenNganh == 0)
            {
                ModelState.AddModelError("MaChuyenNganh", "Vui lòng chọn chuyên ngành.");
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue) return Forbid();
                var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(sinhVien.MaChuyenNganh);
                if (chuyenNganh?.MaKhoa != user.MaKhoa.Value)
                {
                    ModelState.AddModelError("MaChuyenNganh", "Chuyên ngành không thuộc khoa của bạn.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _sinhVienRepository.UpdateAsync(sinhVien);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi sửa sinh viên: {ex.Message}");
                }
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                ViewBag.ChuyenNganhList = khoaId.HasValue
                    ? await _chuyenNganhRepository.GetByKhoaAsync(khoaId.Value)
                    : await _chuyenNganhRepository.GetByKhoaAsync(sinhVien.ChuyenNganh.MaKhoa);
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.KhoaId = user.MaKhoa.Value;
                ViewBag.ChuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(user.MaKhoa.Value);
            }
            return View(sinhVien);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || sinhVien.ChuyenNganh?.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể xóa sinh viên thuộc khoa của mình.");
                }
            }
            return View(sinhVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || sinhVien.ChuyenNganh?.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid();
                }
            }

            try
            {
                await _sinhVienRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi xóa sinh viên: {ex.Message}");
                return View(sinhVien);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || sinhVien.ChuyenNganh?.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể xem sinh viên thuộc khoa của mình.");
                }
            }
            return View(sinhVien);
        }
    }
}