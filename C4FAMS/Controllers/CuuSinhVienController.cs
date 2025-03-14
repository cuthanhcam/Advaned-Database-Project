using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class CuuSinhVienController : Controller
    {
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;
        private readonly ISinhVienRepository _sinhVienRepository;

        public CuuSinhVienController(ICuuSinhVienRepository cuuSinhVienRepository, ISinhVienRepository sinhVienRepository)
        {
            _cuuSinhVienRepository = cuuSinhVienRepository;
            _sinhVienRepository = sinhVienRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(cuuSinhVienList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.SinhVienList = await _sinhVienRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuuSinhVien cuuSinhVien)
        {
            if (ModelState.IsValid)
            {
                await _cuuSinhVienRepository.AddAsync(cuuSinhVien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SinhVienList = await _sinhVienRepository.GetAllAsync();
            return View(cuuSinhVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var cuuSinhVien = await _cuuSinhVienRepository.GetByIdAsync(id);
            if (cuuSinhVien == null) return NotFound();
            ViewBag.SinhVienList = await _sinhVienRepository.GetAllAsync();
            return View(cuuSinhVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CuuSinhVien cuuSinhVien)
        {
            if (id != cuuSinhVien.MSSV) return NotFound();
            if (ModelState.IsValid)
            {
                await _cuuSinhVienRepository.UpdateAsync(cuuSinhVien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SinhVienList = await _sinhVienRepository.GetAllAsync();
            return View(cuuSinhVien);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var cuuSinhVien = await _cuuSinhVienRepository.GetByIdAsync(id);
            if (cuuSinhVien == null) return NotFound();
            return View(cuuSinhVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _cuuSinhVienRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}