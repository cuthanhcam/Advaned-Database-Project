using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KhoaController : Controller
    {
        private readonly IKhoaRepository _khoaRepository;
        private readonly IChuyenNganhRepository _chuyenNganhRepository;

        public KhoaController(IKhoaRepository khoaRepository, IChuyenNganhRepository chuyenNganhRepository)
        {
            _khoaRepository = khoaRepository;
            _chuyenNganhRepository = chuyenNganhRepository;
        }

        public async Task<IActionResult> Index()
        {
            var khoaList = await _khoaRepository.GetAllAsync();
            return View(khoaList);
        }

        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();
            return View(khoa);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null) return NotFound();
            return View(khoa);
        }

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
            var chuyenNganhList = await _chuyenNganhRepository.GetAllByKhoaAsync(id);
            ViewBag.ChuyenNganhList = chuyenNganhList;
            return View(khoa);
        }
    }
}