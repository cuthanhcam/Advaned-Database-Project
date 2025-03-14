using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class SinhVienController : Controller
    {
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly IKhoaRepository _khoaRepository;

        public SinhVienController(ISinhVienRepository sinhVienRepository, IKhoaRepository khoaRepository)
        {
            _sinhVienRepository = sinhVienRepository;
            _khoaRepository = khoaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sinhVienList = await _sinhVienRepository.GetAllAsync();
            return View(sinhVienList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                await _sinhVienRepository.AddAsync(sinhVien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(sinhVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(sinhVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SinhVien sinhVien)
        {
            if (id != sinhVien.MSSV) return NotFound();
            if (ModelState.IsValid)
            {
                await _sinhVienRepository.UpdateAsync(sinhVien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(sinhVien);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
            if (sinhVien == null) return NotFound();
            return View(sinhVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _sinhVienRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}