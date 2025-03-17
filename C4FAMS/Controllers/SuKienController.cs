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
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDangKySuKienRepository _dangKySuKienRepository;

        public SuKienController(
            ISuKienRepository suKienRepository,
            ISuKienChiTietRepository suKienChiTietRepository,
            ISuKienHinhAnhRepository suKienHinhAnhRepository,
            IKhoaRepository khoaRepository,
            UserManager<NguoiDung> userManager,
            IDangKySuKienRepository dangKySuKienRepository)
        {
            _suKienRepository = suKienRepository;
            _suKienChiTietRepository = suKienChiTietRepository;
            _suKienHinhAnhRepository = suKienHinhAnhRepository;
            _khoaRepository = khoaRepository;
            _userManager = userManager;
            _dangKySuKienRepository = dangKySuKienRepository;
        }

        // GET: SuKien/Index
        public async Task<IActionResult> Index(int? khoaId, string sortOrder)
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
                if (!user.MaKhoa.HasValue)
                {
                    TempData["Error"] = "Bạn không thuộc khoa nào.";
                    return RedirectToAction("Index", "Home");
                }
                khoaId = user.MaKhoa.Value;
                ViewBag.KhoaId = khoaId;
                suKienList = await _suKienRepository.GetByKhoaAsync(khoaId.Value);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            // Áp dụng bộ lọc theo khoa
            if (khoaId.HasValue && User.IsInRole("Admin"))
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuKien suKien, SuKienChiTiet suKienChiTiet, List<IFormFile> images)
        {
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

            // Kiểm tra ràng buộc thời gian: NgayToChuc phải từ ngày mai trở đi
            DateTime ngayHienTai = DateTime.Today;
            DateTime ngayToiThieu = ngayHienTai.AddDays(1);
            if (suKien.NgayToChuc < ngayToiThieu)
            {
                ModelState.AddModelError("NgayToChuc", $"Thời gian tổ chức phải từ ngày {ngayToiThieu:dd/MM/yyyy} trở đi.");
            }

            if (ModelState.IsValid)
            {
                await _suKienRepository.AddAsync(suKien);

                suKienChiTiet.MaSuKien = suKien.MaSuKien;
                await _suKienChiTietRepository.AddAsync(suKienChiTiet);

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

                TempData["Success"] = "Tạo sự kiện thành công!";
                return RedirectToAction(nameof(Index));
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
                    TempData["Error"] = "Bạn chỉ có thể sửa sự kiện của khoa mình.";
                    return RedirectToAction(nameof(Index));
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
        // POST: SuKien/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SuKien suKien, SuKienChiTiet suKienChiTiet, List<IFormFile> images)
        {
            if (id != suKien.MaSuKien) return NotFound();

            var existingSuKien = await _suKienRepository.GetByIdAsync(id);
            if (existingSuKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || existingSuKien.MaKhoa != user.MaKhoa.Value) // Sử dụng MaKhoa từ existingSuKien
                {
                    TempData["Error"] = "Bạn chỉ có thể sửa sự kiện của khoa mình.";
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.Remove("MaKhoa");
            ModelState.Remove("suKien.MaKhoa");
            ModelState.Remove("Khoa");
            ModelState.Remove("suKien.Khoa");

            DateTime ngayHienTai = DateTime.Today;
            DateTime ngayToiThieu = ngayHienTai.AddDays(1);
            if (suKien.NgayToChuc < ngayToiThieu)
            {
                ModelState.AddModelError("NgayToChuc", $"Thời gian tổ chức phải từ ngày {ngayToiThieu:dd/MM/yyyy} trở đi.");
            }

            Console.WriteLine($"Received MaKhoa from form: {suKien.MaKhoa}");
            Console.WriteLine($"Existing MaKhoa: {existingSuKien.MaKhoa}");

            if (ModelState.IsValid)
            {
                try
                {
                    existingSuKien.TenSuKien = suKien.TenSuKien;
                    existingSuKien.NgayToChuc = suKien.NgayToChuc;
                    existingSuKien.DiaDiem = suKien.DiaDiem;
                    if (User.IsInRole("Admin")) existingSuKien.MaKhoa = suKien.MaKhoa; // Chỉ Admin thay đổi MaKhoa

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
                                Console.WriteLine($"Saved image: {imagePath}");
                            }
                        }
                        await _suKienRepository.UpdateAsync(existingSuKien);
                    }
                    else
                    {
                        Console.WriteLine("No images uploaded or images is null.");
                    }

                    await _suKienRepository.UpdateAsync(existingSuKien);
                    TempData["Success"] = "Cập nhật sự kiện thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Edit: {ex.Message}");
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
            if (id == null) return NotFound();

            var suKien = await _suKienRepository.GetByIdAsync(id.Value);
            if (suKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    TempData["Error"] = "Bạn chỉ có thể xem sự kiện của khoa mình.";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.KhoaId = user.MaKhoa;
            }

            var suKienChiTiet = await _suKienChiTietRepository.GetByIdAsync(id.Value);
            ViewBag.SuKienChiTiet = suKienChiTiet;

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
                    TempData["Error"] = "Bạn chỉ có thể xóa sự kiện của khoa mình.";
                    return RedirectToAction(nameof(Index));
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
                    TempData["Error"] = "Bạn chỉ có thể xóa sự kiện của khoa mình.";
                    return RedirectToAction(nameof(Index));
                }
            }

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
            TempData["Success"] = "Xóa sự kiện thành công!";
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

            var fileName = Guid.NewGuid().ToString() + extension;
            var savePath = Path.Combine(folderPath, fileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + fileName;
        }

        public async Task<IActionResult> RegisteredCuuSinhVien(int? id)
        {
            if (id == null) return NotFound();

            var suKien = await _suKienRepository.GetByIdAsync(id.Value);
            if (suKien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!user.MaKhoa.HasValue || suKien.MaKhoa != user.MaKhoa.Value)
                {
                    TempData["Error"] = "Bạn chỉ có thể xem danh sách đăng ký của sự kiện thuộc khoa mình.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var dangKyList = await _dangKySuKienRepository.GetBySuKienAsync(id.Value);

            var registeredUsers = new List<object>();
            foreach (var dangKy in dangKyList.Where(d => d.TrangThai))
            {
                var hoTen = dangKy.CuuSinhVien?.SinhVien?.HoTen ?? "Không xác định";
                var nguoiDung = await _userManager.Users.FirstOrDefaultAsync(u => u.MSSV == dangKy.MSSV);
                registeredUsers.Add(new
                {
                    MSSV = dangKy.MSSV,
                    HoTen = hoTen,
                    Email = nguoiDung?.Email ?? "Không có email",
                    NgayDangKy = dangKy.NgayDangKy
                });
            }

            ViewBag.SuKien = suKien;
            ViewBag.RegisteredUsers = registeredUsers;
            return View();
        }
    }
}