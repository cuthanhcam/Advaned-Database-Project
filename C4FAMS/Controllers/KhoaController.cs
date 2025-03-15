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
                var khoa = await _khoaRepository.GetByIdAsync(user.MaKhoa  ?? 0);
                if (khoa == null) return NotFound("Bạn không thuộc khoa nào.");
                return View(new List<Khoa> { khoa }); // Chỉ hiển thị khoa của họ
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
            await _khoaRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.MaKhoa != id) return Forbid("Bạn chỉ có thể xem thông tin khoa của mình.");
            }

            var chuyenNganhList = await _chuyenNganhRepository.GetAllByKhoaAsync(id);
            ViewBag.ChuyenNganhList = chuyenNganhList;
            return View(khoa);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateChuyenNganh(int maKhoa)
        {
            var khoa = _khoaRepository.GetByIdAsync(maKhoa).Result;
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

            await _chuyenNganhRepository.AddAsync(chuyenNganh);
            return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
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
            if (id != chuyenNganh.MaChuyenNganh) return NotFound();

            if (ModelState.IsValid)
            {
                await _chuyenNganhRepository.UpdateAsync(chuyenNganh);
                return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
            }

            await _chuyenNganhRepository.UpdateAsync(chuyenNganh);
            return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChuyenNganh(int id)
        {
            var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(id);
            if (chuyenNganh == null) return NotFound();
            return View(chuyenNganh);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteChuyenNganh")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChuyenNganhConfirmed(int id)
        {
            var chuyenNganh = await _chuyenNganhRepository.GetByIdAsync(id);
            if (chuyenNganh == null) return NotFound();
            await _chuyenNganhRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Details), new { id = chuyenNganh.MaKhoa });
        }
    }
}