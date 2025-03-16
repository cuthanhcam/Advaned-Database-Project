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
        private readonly UserManager<NguoiDung> _userManager;

        public KhoaController(IKhoaRepository khoaRepository, IChuyenNganhRepository chuyenNganhRepository, UserManager<NguoiDung> userManager)
        {
            _khoaRepository = khoaRepository;
            _chuyenNganhRepository = chuyenNganhRepository;
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
            return Forbid();
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
                await _khoaRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var khoa = await _khoaRepository.GetByIdAsync(id);
                return View("Delete", khoa);
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
                    return Forbid("Bạn chỉ có thể xem thông tin khoa của mình.");
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
    }
}