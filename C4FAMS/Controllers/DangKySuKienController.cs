using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, CuuSinhVien")]
    public class DangKySuKienController : Controller
    {
        private readonly IDangKySuKienRepository _dangKySuKienRepository;
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;
        private readonly ISuKienRepository _suKienRepository;

        public DangKySuKienController(IDangKySuKienRepository dangKySuKienRepository,
            ICuuSinhVienRepository cuuSinhVienRepository, ISuKienRepository suKienRepository)
        {
            _dangKySuKienRepository = dangKySuKienRepository;
            _cuuSinhVienRepository = cuuSinhVienRepository;
            _suKienRepository = suKienRepository;
        }

        public async Task<IActionResult> Index()
        {
            var dangKyList = await _dangKySuKienRepository.GetAllAsync();
            return View(dangKyList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            ViewBag.SuKienList = await _suKienRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DangKySuKien dangKySuKien)
        {
            if (ModelState.IsValid)
            {
                dangKySuKien.NgayDangKy = DateTime.Now; // Tự động gán ngày hiện tại
                await _dangKySuKienRepository.AddAsync(dangKySuKien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            ViewBag.SuKienList = await _suKienRepository.GetAllAsync();
            return View(dangKySuKien);
        }

        // Không có chức năng Edit vì thông tin đăng ký sự kiện không thể chỉnh sửa

        public async Task<IActionResult> Delete(int id)
        {
            var dangKySuKien = await _dangKySuKienRepository.GetByIdAsync(id);
            if (dangKySuKien == null) return NotFound();
            return View(dangKySuKien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dangKySuKienRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}