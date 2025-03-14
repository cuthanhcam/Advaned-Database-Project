using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, CuuSinhVien")]
    public class ThanhTuuController : Controller
    {
        private readonly IThanhTuuRepository _thanhTuuRepository;
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;

        public ThanhTuuController(IThanhTuuRepository thanhTuuRepository, ICuuSinhVienRepository cuuSinhVienRepository)
        {
            _thanhTuuRepository = thanhTuuRepository;
            _cuuSinhVienRepository = cuuSinhVienRepository;
        }

        public async Task<IActionResult> Index()
        {
            var thanhTuuList = await _thanhTuuRepository.GetAllAsync();
            return View(thanhTuuList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhTuu thanhTuu)
        {
            if (ModelState.IsValid)
            {
                await _thanhTuuRepository.AddAsync(thanhTuu);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thanhTuu);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var thanhTuu = await _thanhTuuRepository.GetByIdAsync(id);
            if (thanhTuu == null) return NotFound();
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thanhTuu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThanhTuu thanhTuu)
        {
            if (id != thanhTuu.MaThanhTuu) return NotFound();
            if (ModelState.IsValid)
            {
                await _thanhTuuRepository.UpdateAsync(thanhTuu);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thanhTuu);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var thanhTuu = await _thanhTuuRepository.GetByIdAsync(id);
            if (thanhTuu == null) return NotFound();
            return View(thanhTuu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _thanhTuuRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}