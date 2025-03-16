using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class KhoaController : Controller
    {
        private readonly IKhoaRepository _khoaRepository;
        private readonly IChuyenNganhRepository _chuyenNganhRepository;
        private readonly ISuKienRepository _suKienRepository;
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly UserManager<NguoiDung> _userManager;

        public KhoaController(
            IKhoaRepository khoaRepository,
            IChuyenNganhRepository chuyenNganhRepository,
            ISuKienRepository suKienRepository,
            ISinhVienRepository sinhVienRepository,
            UserManager<NguoiDung> userManager)
        {
            _khoaRepository = khoaRepository;
            _chuyenNganhRepository = chuyenNganhRepository;
            _suKienRepository = suKienRepository;
            _sinhVienRepository = sinhVienRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                return View(khoaList);
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue) return NotFound("Bạn không thuộc khoa nào.");
                var khoa = await _khoaRepository.GetByIdAsync(user.MaKhoa.Value);
                if (khoa == null) return NotFound("Khoa không tồn tại.");
                return View(new List<Khoa> { khoa });
            }
            return StatusCode(403, "Bạn không có quyền truy cập.");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                await _khoaRepository.AddAsync(khoa);
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Khoa khoa)
        {
            if (id != khoa.MaKhoa) return NotFound();
            if (ModelState.IsValid)
            {
                await _khoaRepository.UpdateAsync(khoa);
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var khoa = await _khoaRepository.GetByIdAsync(id);
                if (khoa == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem khoa có được sử dụng trong SuKien không
                var suKienList = await _suKienRepository.GetByKhoaAsync(id);
                if (suKienList.Any())
                {
                    ModelState.AddModelError("", "Không thể xóa khoa này vì có sự kiện đang sử dụng.");
                    return View("Delete", khoa);
                }

                await _khoaRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var khoa = await _khoaRepository.GetByIdAsync(id);
                return View("Delete", khoa);
            }
            catch (Exception)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi xóa khoa.");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || user.MaKhoa != id)
                {
                    return StatusCode(403, "Bạn chỉ có thể xem thông tin khoa của mình.");
                }
            }

            var chuyenNganhList = await _chuyenNganhRepository.GetByKhoaAsync(id);
            ViewBag.ChuyenNganhList = chuyenNganhList;
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateChuyenNganh(int maKhoa)
        {
            var khoa = await _khoaRepository.GetByIdAsync(maKhoa);
            if (khoa == null) return NotFound();
            var chuyenNganh = new ChuyenNganh { MaKhoa = maKhoa };
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChuyenNganh(ChuyenNganh chuyenNganh)
        {
            if (ModelState.IsValid)
            {
                await _chuyenNganhRepository.AddAsync(chuyenNganh);
                return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
            }
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditChuyenNganh(int id)
        {
            var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(id);
            if (chuyenNganh == null) return NotFound();
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChuyenNganh(int id, ChuyenNganh chuyenNganh)
        {
            if (id != chuyenNganh.MaChuyenNganh) return NotFound("Chuyên ngành không tồn tại.");
            if (ModelState.IsValid)
            {
                await _chuyenNganhRepository.UpdateAsync(chuyenNganh);
                return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
            }
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChuyenNganh(int id)
        {
            var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(id);
            if (chuyenNganh == null) return NotFound("Chuyên ngành không tồn tại.");
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteChuyenNganh")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChuyenNganhConfirmed(int id)
        {
            try
            {
                var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(id);
                if (chuyenNganh == null)
                {
                    return NotFound("Chuyên ngành không tồn tại.");
                }

                // Kiểm tra xem chuyên ngành có được sử dụng trong SinhVien không
                var sinhVienList = await _sinhVienRepository.GetByChuyenNganhAsync(id);
                if (sinhVienList.Any())
                {
                    ModelState.AddModelError("", "Không thể xóa chuyên ngành này vì có sinh viên đang sử dụng.");
                    return View("DeleteChuyenNganh", chuyenNganh);
                }

                await _chuyenNganhRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
            }
            catch (Exception)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi xóa chuyên ngành.");
            }
        }
    }
}