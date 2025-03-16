using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace C4FAMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISuKienRepository _suKienRepository;
        private readonly IKhoaRepository _khoaRepository;
        private readonly IDangKySuKienRepository _dangKySuKienRepository;

        public HomeController(
            ISuKienRepository suKienRepository,
            IKhoaRepository khoaRepository,
            IDangKySuKienRepository dangKySuKienRepository)
        {
            _suKienRepository = suKienRepository;
            _khoaRepository = khoaRepository;
            _dangKySuKienRepository = dangKySuKienRepository;
        }

        public async Task<IActionResult> Index(int? khoaId, string sortOrder)
        {
            var suKienList = await _suKienRepository.GetAllAsync();
            var khoaList = await _khoaRepository.GetAllAsync();
            ViewBag.KhoaList = khoaList;

            // Áp dụng bộ lọc theo khoa
            if (khoaId.HasValue)
            {
                suKienList = suKienList.Where(s => s.MaKhoa == khoaId.Value);
            }

            // Xử lý sắp xếp
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
            return View("IndexCuuSinhVien", suKienList); // Dùng view IndexCuuSinhVien cho tất cả
        }

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

            var suKienChiTiet = await _suKienRepository.GetChiTietByIdAsync(id.Value); // Giả sử có method này
            ViewBag.SuKienChiTiet = suKienChiTiet;
            return View(suKien);
        }

        public IActionResult StatusCode(int code)
        {
            switch (code)
            {
                case 404:
                    return View("~/Views/Shared/NotFound.cshtml", "Trang không tồn tại.");
                case 403:
                    return View("~/Views/Shared/AccessDenied.cshtml", "Bạn không có quyền truy cập.");
                default:
                    return View("~/Views/Shared/Error.cshtml", $"Lỗi {code}: Đã xảy ra lỗi không xác định.");
            }
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", "Đã xảy ra lỗi trong hệ thống.");
        }
    }
}