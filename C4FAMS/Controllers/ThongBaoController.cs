using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoRepository _thongBaoRepository;
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;

        public ThongBaoController(IThongBaoRepository thongBaoRepository, ICuuSinhVienRepository cuuSinhVienRepository)
        {
            _thongBaoRepository = thongBaoRepository;
            _cuuSinhVienRepository = cuuSinhVienRepository;
        }

        public async Task<IActionResult> Index()
        {
            var thongBaoList = await _thongBaoRepository.GetAllAsync();
            return View(thongBaoList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                thongBao.NgayGui = DateTime.Now;
                await _thongBaoRepository.AddAsync(thongBao);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thongBao);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var thongBao = await _thongBaoRepository.GetByIdAsync(id);
            if (thongBao == null) return NotFound();
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thongBao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThongBao thongBao)
        {
            if (id != thongBao.MaThongBao) return NotFound();
            if (ModelState.IsValid)
            {
                await _thongBaoRepository.UpdateAsync(thongBao);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuuSinhVienList = await _cuuSinhVienRepository.GetAllAsync();
            return View(thongBao);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var thongBao = await _thongBaoRepository.GetByIdAsync(id);
            if (thongBao == null) return NotFound();
            return View(thongBao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _thongBaoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}