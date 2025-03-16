using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace C4FAMS.Controllers
{
    [Authorize(Roles = "Admin, Khoa")]
    public class SuKienController : Controller
    {
        private readonly ISuKienRepository _suKienRepository;
        private readonly ISuKienChiTietRepository _suKienChiTietRepository;
        private readonly ISuKienHinhAnhRepository _suKienHinhAnhRepository;
        private readonly IKhoaRepository _khoaRepository;
        private readonly UserManager<NguoiDung> _userManager;

        public SuKienController(
            ISuKienRepository suKienRepository,
            ISuKienChiTietRepository suKienChiTietRepository,
            ISuKienHinhAnhRepository suKienHinhAnhRepository,
            IKhoaRepository khoaRepository,
            UserManager<NguoiDung> userManager)
        {
            _suKienRepository = suKienRepository;
            _suKienChiTietRepository = suKienChiTietRepository;
            _suKienHinhAnhRepository = suKienHinhAnhRepository;
            _khoaRepository = khoaRepository;
            _userManager = userManager;
        }

        // GET: SuKien/Index
        public async Task<IActionResult> Index(int? khoaId, DateTime? ngayToChuc)
        {
            IEnumerable<SuKien> suKienList;

            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;
                suKienList = await _suKienRepository.GetAllAsync();
            }
            else if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue) return Forbid("Bạn không thuộc khoa nào.");
                khoaId = user.MaKhoa.Value;
                ViewBag.KhoaId = khoaId;
                suKienList = await _suKienRepository.GetByKhoaAsync(khoaId.Value);
            }
            else
            {
                return Forbid();
            }

            // Áp dụng bộ lọc
            if (khoaId.HasValue)
            {
                suKienList = suKienList.Where(s => s.MaKhoa == khoaId.Value);
            }

            if (ngayToChuc.HasValue)
            {
                suKienList = suKienList.Where(s => s.NgayToChuc.Date == ngayToChuc.Value.Date);
            }

            ViewBag.SelectedKhoaId = khoaId;
            ViewBag.SelectedNgayToChuc = ngayToChuc?.ToString("yyyy-MM-dd");

            return View(suKienList);
        }

        // GET: SuKien/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;
            }
            return View();
        }

        // POST: SuKien/Create
        // POST: SuKien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuKien suKien, SuKienChiTiet suKienChiTiet, List<IFormFile> images)
        {
            // Gán MaKhoa trước khi kiểm tra validation
            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue)
                {
                    ModelState.AddModelError("", "Bạn không thuộc khoa nào.");
                    return View(suKien);
                }
                suKien.MaKhoa = user.MaKhoa.Value;
            }

            // Xóa lỗi validation liên quan đến MaKhoa và Khoa (cho cả Admin và Khoa)
            ModelState.Remove("MaKhoa");
            ModelState.Remove("suKien.MaKhoa");
            ModelState.Remove("Khoa");
            ModelState.Remove("suKien.Khoa");

            // Kiểm tra ràng buộc thời gian: NgayToChuc phải từ ngày hôm nay trở đi
            DateTime ngayHienTai = DateTime.Today; // Lấy ngày hiện tại (không tính giờ)
            if (suKien.NgayToChuc < ngayHienTai)
            {
                ModelState.AddModelError("NgayToChuc", "Thời gian tổ chức phải từ ngày hôm nay trở đi.");
            }

            // Kiểm tra dữ liệu gửi lên từ form trước khi validation
            Console.WriteLine($"Form MaKhoa before validation: {suKien.MaKhoa}");
            Console.WriteLine($"Form NgayToChuc before validation: {suKien.NgayToChuc}");

            if (ModelState.IsValid)
            {
                // Thêm SuKien
                await _suKienRepository.AddAsync(suKien);

                // Thêm SuKienChiTiet
                suKienChiTiet.MaSuKien = suKien.MaSuKien;
                await _suKienChiTietRepository.AddAsync(suKienChiTiet);

                // Xử lý upload hình ảnh
                if (images != null && images.Any())
                {
                    suKien.SuKienHinhAnhs ??= new List<SuKienHinhAnh>();
                    foreach (var image in images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var imagePath = await SaveImage(image);
                            suKien.SuKienHinhAnhs.Add(new SuKienHinhAnh
                            {
                                MaSuKien = suKien.MaSuKien,
                                HinhAnh = imagePath
                            });
                        }
                    }
                    await _suKienRepository.UpdateAsync(suKien);
                }

                return RedirectToAction(nameof(Index));
            }

            // Log lỗi validation chi tiết
            foreach (var error in ModelState)
            {
                Console.WriteLine($"Validation Error - Key: {error.Key}, Error: {error.Value.Errors.FirstOrDefault()?.ErrorMessage}");
            }

            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;
            }
            return View(suKien);
        }

        // GET: SuKien/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var suKien = await _suKienRepository.GetByIdAsync(id.Value);
            if (suKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể sửa sự kiện của khoa mình.");
                }
            }

            var suKienChiTiet = await _suKienChiTietRepository.GetByIdAsync(id.Value);
            ViewBag.SuKienChiTiet = suKienChiTiet;

            if (User.IsInRole("Admin"))
            {
                var khoaList = await _khoaRepository.GetAllAsync();
                ViewBag.KhoaList = khoaList;
            }

            return View(suKien);
        }

        // POST: SuKien/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SuKien suKien, SuKienChiTiet suKienChiTiet, List<IFormFile> images)
        {
            if (id != suKien.MaSuKien) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể sửa sự kiện của khoa mình.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSuKien = await _suKienRepository.GetByIdAsync(id);
                    if (existingSuKien == null) return NotFound();

                    // Cập nhật thông tin SuKien
                    existingSuKien.TenSuKien = suKien.TenSuKien;
                    existingSuKien.NgayToChuc = suKien.NgayToChuc;
                    existingSuKien.DiaDiem = suKien.DiaDiem;
                    if (User.IsInRole("Admin")) existingSuKien.MaKhoa = suKien.MaKhoa;

                    // Cập nhật SuKienChiTiet
                    var existingChiTiet = await _suKienChiTietRepository.GetByIdAsync(id);
                    if (existingChiTiet != null)
                    {
                        existingChiTiet.MoTa = suKienChiTiet.MoTa;
                        existingChiTiet.NoiDung = suKienChiTiet.NoiDung;
                        await _suKienChiTietRepository.UpdateAsync(existingChiTiet);
                    }
                    else
                    {
                        suKienChiTiet.MaSuKien = id;
                        await _suKienChiTietRepository.AddAsync(suKienChiTiet);
                    }

                    // Xử lý upload nhiều hình ảnh mới
                    if (images != null && images.Any())
                    {
                        Console.WriteLine($"Number of images uploaded: {images.Count}");
                        existingSuKien.SuKienHinhAnhs ??= new List<SuKienHinhAnh>();
                        foreach (var image in images)
                        {
                            if (image != null && image.Length > 0)
                            {
                                var imagePath = await SaveImage(image);
                                existingSuKien.SuKienHinhAnhs.Add(new SuKienHinhAnh
                                {
                                    MaSuKien = existingSuKien.MaSuKien,
                                    HinhAnh = imagePath
                                });
                            }
                        }
                        await _suKienRepository.UpdateAsync(existingSuKien); // Đảm bảo lưu danh sách ảnh
                    }

                    await _suKienRepository.UpdateAsync(existingSuKien);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật sự kiện: {ex.Message}");
                }
            }

            var khoaList = await _khoaRepository.GetAllAsync();
            ViewBag.KhoaList = khoaList;
            ViewBag.SuKienChiTiet = suKienChiTiet;
            return View(suKien);
        }

        // GET: SuKien/Details
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

            var suKienChiTiet = await _suKienChiTietRepository.GetByIdAsync(id.Value);
            ViewBag.SuKienChiTiet = suKienChiTiet;

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.KhoaId = user.MaKhoa;
            }

            return View(suKien);
        }

        // GET: SuKien/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var suKien = await _suKienRepository.GetByIdAsync(id.Value);
            if (suKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể xóa sự kiện của khoa mình.");
                }
            }

            return View(suKien);
        }

        // POST: SuKien/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    return Forbid("Bạn chỉ có thể xóa sự kiện của khoa mình.");
                }
            }

            // Xóa file hình ảnh từ wwwroot
            if (suKien.SuKienHinhAnhs != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                foreach (var hinhAnh in suKien.SuKienHinhAnhs)
                {
                    var filePath = Path.Combine(folderPath, hinhAnh.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            await _suKienChiTietRepository.DeleteAsync(id);
            await _suKienRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Định dạng file không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png.");
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Tạo tên file duy nhất để tránh ghi đè
            var fileName = Guid.NewGuid().ToString() + extension;
            var savePath = Path.Combine(folderPath, fileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + fileName;
        }
    }
}