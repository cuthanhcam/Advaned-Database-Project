using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "CuuSinhVien")]
    public class CuuSinhVienSuKienController : Controller
    {
        private readonly ISuKienRepository _suKienRepository;
        private readonly IKhoaRepository _khoaRepository;
        private readonly IDangKySuKienRepository _dangKySuKienRepository;
        private readonly UserManager<NguoiDung> _userManager;

        public CuuSinhVienSuKienController(
            ISuKienRepository suKienRepository,
            IKhoaRepository khoaRepository,
            IDangKySuKienRepository dangKySuKienRepository,
            UserManager<NguoiDung> userManager)
        {
            _suKienRepository = suKienRepository;
            _khoaRepository = khoaRepository;
            _dangKySuKienRepository = dangKySuKienRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? khoaId, string sortOrder)
        {
            var suKienList = await _suKienRepository.GetAllAsync();
            var khoaList = await _khoaRepository.GetAllAsync();
            ViewBag.KhoaList = khoaList;

            if (khoaId.HasValue)
            {
                suKienList = suKienList.Where(s => s.MaKhoa == khoaId.Value);
            }

            sortOrder = string.IsNullOrEmpty(sortOrder) ? "nearest" : sortOrder;
            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "nearest":
                    suKienList = suKienList.OrderBy(s => s.NgayToChuc);
                    break;
                case "farthest":
                    suKienList = suKienList.OrderByDescending(s => s.NgayToChuc);
                    break;
                default:
                    suKienList = suKienList.OrderBy(s => s.NgayToChuc);
                    break;
            }

            ViewBag.SelectedKhoaId = khoaId;
            return View(suKienList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(int maSuKien)
        {
            var suKien = await _suKienRepository.GetByIdAsync(maSuKien);
            if (suKien == null)
            {
                return NotFound("Sự kiện không tồn tại.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user.MSSV))
            {
                return BadRequest("Không tìm thấy thông tin MSSV của bạn.");
            }

            var existingDangKy = await _dangKySuKienRepository.GetByCuuSinhVienAsync(user.MSSV);
            if (existingDangKy.Any(d => d.MaSuKien == maSuKien && d.TrangThai))
            {
                TempData["Error"] = "Bạn đã đăng ký sự kiện này rồi.";
                return RedirectToAction("Index");
            }

            if (suKien.NgayToChuc < DateTime.Now)
            {
                TempData["Error"] = "Không thể đăng ký sự kiện đã qua thời gian tổ chức.";
                return RedirectToAction("Index");
            }

            var dangKy = new DangKySuKien
            {
                MaSuKien = maSuKien,
                MSSV = user.MSSV,
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };

            await _dangKySuKienRepository.AddAsync(dangKy);
            TempData["Success"] = "Đăng ký sự kiện thành công!";
            return RedirectToAction("Index");
        }

        // Thêm action Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suKien = await _suKienRepository.GetByIdAsync(id.Value);
            if (suKien == null)
            {
                return NotFound();
            }

            var suKienChiTiet = await _suKienRepository.GetChiTietByIdAsync(id.Value); // Giả sử bạn có method này trong ISuKienRepository
            ViewBag.SuKienChiTiet = suKienChiTiet;
            return View(suKien);
        }

        // Thêm vào cuối Controller
        public async Task<IActionResult> MyEvents()
        {
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user.MSSV))
            {
                return BadRequest("Không tìm thấy thông tin MSSV của bạn.");
            }

            var dangKyList = await _dangKySuKienRepository.GetByCuuSinhVienAsync(user.MSSV);
            return View(dangKyList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int maDangKy)
        {
            var dangKy = await _dangKySuKienRepository.GetByIdAsync(maDangKy);
            if (dangKy == null)
            {
                return NotFound("Không tìm thấy đăng ký.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (dangKy.MSSV != user.MSSV)
            {
                return Forbid("Bạn không có quyền hủy đăng ký này.");
            }

            var suKien = await _suKienRepository.GetByIdAsync(dangKy.MaSuKien);
            if (suKien.NgayToChuc < DateTime.Now)
            {
                TempData["Error"] = "Không thể hủy đăng ký sự kiện đã diễn ra.";
                return RedirectToAction("MyEvents");
            }

            dangKy.TrangThai = false;
            await _dangKySuKienRepository.UpdateAsync(dangKy);
            TempData["Success"] = "Hủy đăng ký thành công!";
            return RedirectToAction("MyEvents");
        }
    }
}