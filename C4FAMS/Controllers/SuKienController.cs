using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class SuKienController : Controller
    {
        private readonly ISuKienRepository _suKienRepository;
        private readonly IKhoaRepository _khoaRepository;

        public SuKienController(ISuKienRepository suKienRepository, IKhoaRepository khoaRepository)
        {
            _suKienRepository = suKienRepository;
            _khoaRepository = khoaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var suKienList = await _suKienRepository.GetAllAsync();
            return View(suKienList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuKien suKien)
        {
            if (ModelState.IsValid)
            {
                await _suKienRepository.AddAsync(suKien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(suKien);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(suKien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SuKien suKien)
        {
            if (id != suKien.MaSuKien) return NotFound();
            if (ModelState.IsValid)
            {
                await _suKienRepository.UpdateAsync(suKien);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(suKien);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();
            return View(suKien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _suKienRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}