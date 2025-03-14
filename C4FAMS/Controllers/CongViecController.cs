using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, CuuSinhVien")]
    public class CongViecController : Controller
    {
        private readonly ICongViecRepository _congViecRepository;
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;

        public CongViecController(ICongViecRepository congViecRepository, ICuuSinhVienRepository cuuSinhVienRepository)
        {
            _congViecRepository = congViecRepository;
            _cuuSinhVienRepository = cuuSinhVienRepository;
        }

        public async Task<IActionResult> Index()
        {
            var congViecList = await _congViecRepository.GetAllAsync();
            return View(congViecList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CongViec congViec)
        {
            if (ModelState.IsValid)
            {
                await _congViecRepository.AddAsync(congViec);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(congViec);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var congViec = await _congViecRepository.GetByIdAsync(id);
            if (congViec == null) return NotFound();
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(congViec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CongViec congViec)
        {
            if (id != congViec.MaCongViec) return NotFound();
            if (ModelState.IsValid)
            {
                await _congViecRepository.UpdateAsync(congViec);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(congViec);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var congViec = await _congViecRepository.GetByIdAsync(id);
            if (congViec == null) return NotFound();
            return View(congViec);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _congViecRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}