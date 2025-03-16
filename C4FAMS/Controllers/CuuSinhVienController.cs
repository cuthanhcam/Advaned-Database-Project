// Controllers/CuuSinhVienController.cs
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace C4FAMS.Controllers
{
    public class CuuSinhVienController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly ICuuSinhVienRepository _cuuSinhVienRepository;
        private readonly ICongViecRepository _congViecRepository;
        private readonly IThanhTuuRepository _thanhTuuRepository;
        private readonly ISinhVienRepository _sinhVienRepository;

        public CuuSinhVienController(
            UserManager<NguoiDung> userManager,
            ICuuSinhVienRepository cuuSinhVienRepository,
            ICongViecRepository congViecRepository,
            IThanhTuuRepository thanhTuuRepository,
            ISinhVienRepository sinhVienRepository)
        {
            _userManager = userManager;
            _cuuSinhVienRepository = cuuSinhVienRepository;
            _congViecRepository = congViecRepository;
            _thanhTuuRepository = thanhTuuRepository;
            _sinhVienRepository = sinhVienRepository;
        }

        // Controllers/CuuSinhVienController.cs
        [Authorize(Roles = "CuuSinhVien")]
        public async Task<IActionResult> ManageProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV))
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(user.MSSV);
            if (cuuSinhVien == null)
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            var model = new ManageCuuSinhVienViewModel
            {
                NamTotNghiep = cuuSinhVien.NamTotNghiep,
                XepLoaiTotNghiep = cuuSinhVien.XepLoaiTotNghiep,
                DiaChiHienTai = cuuSinhVien.DiaChiHienTai,
                HinhThucLienLac = cuuSinhVien.HinhThucLienLac,
                CongViecs = (await _congViecRepository.GetByMssvAsync(user.MSSV)).Select(cv => new CongViecViewModel
                {
                    MaCongViec = cv.MaCongViec, // Ánh xạ khóa chính
                    ViTri = cv.ViTri,
                    CongTy = cv.CongTy,
                    LinhVuc = cv.LinhVuc,
                    ThuNhap = cv.ThuNhap
                }),
                ThanhTuus = (await _thanhTuuRepository.GetByMssvAsync(user.MSSV)).Select(tt => new ThanhTuuViewModel
                {
                    MaThanhTuu = tt.MaThanhTuu, // Ánh xạ khóa chính
                    TenThanhTuu = tt.TenThanhTuu,
                    MoTa = tt.MoTa,
                    NamDatDuoc = tt.NamDatDuoc
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageProfile(ManageCuuSinhVienViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV))
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(user.MSSV);
            if (cuuSinhVien == null)
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            if (ModelState.IsValid)
            {
                cuuSinhVien.NamTotNghiep = model.NamTotNghiep;
                cuuSinhVien.XepLoaiTotNghiep = model.XepLoaiTotNghiep;
                cuuSinhVien.DiaChiHienTai = model.DiaChiHienTai;
                cuuSinhVien.HinhThucLienLac = model.HinhThucLienLac;

                await _cuuSinhVienRepository.UpdateAsync(cuuSinhVien);

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(ManageProfile));
            }

            // Gán lại danh sách công việc và thành tựu khi có lỗi
            model.CongViecs = (await _congViecRepository.GetByMssvAsync(user.MSSV)).Select(cv => new CongViecViewModel
            {
                MaCongViec = cv.MaCongViec,
                ViTri = cv.ViTri,
                CongTy = cv.CongTy,
                LinhVuc = cv.LinhVuc,
                ThuNhap = cv.ThuNhap
            });
            model.ThanhTuus = (await _thanhTuuRepository.GetByMssvAsync(user.MSSV)).Select(tt => new ThanhTuuViewModel
            {
                MaThanhTuu = tt.MaThanhTuu,
                TenThanhTuu = tt.TenThanhTuu,
                MoTa = tt.MoTa,
                NamDatDuoc = tt.NamDatDuoc
            });

            return View(model);
        }

        // GET: CuuSinhVien/AddJob
        [Authorize(Roles = "CuuSinhVien")]
        public IActionResult AddJob()
        {
            return View(new CongViecViewModel());
        }

        // POST: CuuSinhVien/AddJob
        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJob(CongViecViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.MSSV == null)
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            if (ModelState.IsValid)
            {
                var congViec = new CongViec
                {
                    MSSV = user.MSSV,
                    ViTri = model.ViTri,
                    CongTy = model.CongTy,
                    LinhVuc = model.LinhVuc,
                    ThuNhap = model.ThuNhap
                };

                await _congViecRepository.AddAsync(congViec);
                return RedirectToAction(nameof(ManageProfile));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "CuuSinhVien")]
        public async Task<IActionResult> EditJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var congViec = await _congViecRepository.GetByIdAsync(id);
            if (congViec == null || congViec.MSSV != user.MSSV) return NotFound();

            var model = new CongViecViewModel
            {
                MaCongViec = congViec.MaCongViec,
                ViTri = congViec.ViTri,
                CongTy = congViec.CongTy,
                LinhVuc = congViec.LinhVuc,
                ThuNhap = congViec.ThuNhap
            };

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob(int id, CongViecViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var congViec = await _congViecRepository.GetByIdAsync(id);
            if (congViec == null || congViec.MSSV != user.MSSV) return NotFound();

            if (ModelState.IsValid)
            {
                congViec.ViTri = model.ViTri;
                congViec.CongTy = model.CongTy;
                congViec.LinhVuc = model.LinhVuc;
                congViec.ThuNhap = model.ThuNhap;

                await _congViecRepository.UpdateAsync(congViec);
                TempData["SuccessMessage"] = "Cập nhật công việc thành công.";
                return RedirectToAction(nameof(ManageProfile));
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var congViec = await _congViecRepository.GetByIdAsync(id);
            if (congViec == null || congViec.MSSV != user.MSSV)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc hoặc bạn không có quyền xóa.";
                return RedirectToAction(nameof(ManageProfile));
            }

            await _congViecRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa công việc thành công.";
            return RedirectToAction(nameof(ManageProfile));
        }

        // POST: CuuSinhVien/DeleteJobConfirmed
        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJobConfirmed(int id)
        {
            var job = await _congViecRepository.GetByIdAsync(id);
            if (job == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.MSSV != job.MSSV) return Forbid();

            await _congViecRepository.DeleteAsync(id);
            return RedirectToAction(nameof(ManageProfile));
        }

        // GET: CuuSinhVien/AddAchievement
        [Authorize(Roles = "CuuSinhVien")]
        public IActionResult AddAchievement()
        {
            return View(new ThanhTuuViewModel());
        }

        // POST: CuuSinhVien/AddAchievement
        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAchievement(ThanhTuuViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.MSSV == null)
            {
                return NotFound("Không tìm thấy thông tin cựu sinh viên.");
            }

            if (ModelState.IsValid)
            {
                var thanhTuu = new ThanhTuu
                {
                    MSSV = user.MSSV,
                    TenThanhTuu = model.TenThanhTuu,
                    MoTa = model.MoTa,
                    NamDatDuoc = model.NamDatDuoc
                };

                await _thanhTuuRepository.AddAsync(thanhTuu);
                return RedirectToAction(nameof(ManageProfile));
            }

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "CuuSinhVien")]
        public async Task<IActionResult> EditAchievement(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var thanhTuu = await _thanhTuuRepository.GetByIdAsync(id);
            if (thanhTuu == null || thanhTuu.MSSV != user.MSSV) return NotFound();

            var model = new ThanhTuuViewModel
            {
                MaThanhTuu = thanhTuu.MaThanhTuu,
                TenThanhTuu = thanhTuu.TenThanhTuu,
                MoTa = thanhTuu.MoTa,
                NamDatDuoc = thanhTuu.NamDatDuoc
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAchievement(int id, ThanhTuuViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var thanhTuu = await _thanhTuuRepository.GetByIdAsync(id);
            if (thanhTuu == null || thanhTuu.MSSV != user.MSSV) return NotFound();

            if (ModelState.IsValid)
            {
                thanhTuu.TenThanhTuu = model.TenThanhTuu;
                thanhTuu.MoTa = model.MoTa;
                thanhTuu.NamDatDuoc = model.NamDatDuoc;

                await _thanhTuuRepository.UpdateAsync(thanhTuu);
                TempData["SuccessMessage"] = "Cập nhật thành tựu thành công.";
                return RedirectToAction(nameof(ManageProfile));
            }

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.MSSV)) return NotFound();

            var thanhTuu = await _thanhTuuRepository.GetByIdAsync(id);
            if (thanhTuu == null || thanhTuu.MSSV != user.MSSV)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thành tựu hoặc bạn không có quyền xóa.";
                return RedirectToAction(nameof(ManageProfile));
            }

            await _thanhTuuRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa thành tựu thành công.";
            return RedirectToAction(nameof(ManageProfile));
        }

        // POST: CuuSinhVien/DeleteAchievementConfirmed
        [HttpPost]
        [Authorize(Roles = "CuuSinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAchievementConfirmed(int id)
        {
            var achievement = await _thanhTuuRepository.GetByIdAsync(id);
            if (achievement == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.MSSV != achievement.MSSV) return Forbid();

            await _thanhTuuRepository.DeleteAsync(id);
            return RedirectToAction(nameof(ManageProfile));
        }

        // GET: CuuSinhVien/Index (Admin/Khoa)
        [Authorize(Roles = "Admin,Khoa")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            IEnumerable<CuuSinhVien> cuuSinhViens;

            if (User.IsInRole("Khoa") && currentUser?.MaKhoa.HasValue == true)
            {
                cuuSinhViens = await _cuuSinhVienRepository.GetByKhoaAsync(currentUser.MaKhoa.Value);
            }
            else
            {
                cuuSinhViens = await _cuuSinhVienRepository.GetAllAsync();
            }

            return View(cuuSinhViens);
        }

        [Authorize(Roles = "Admin,Khoa")]
        public async Task<IActionResult> Details(string mssv)
        {
            if (string.IsNullOrEmpty(mssv)) return NotFound();

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(mssv);
            if (cuuSinhVien == null)
            {
                return NotFound("Không tìm thấy cựu sinh viên.");
            }

            if (User.IsInRole("Khoa"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || !currentUser.MaKhoa.HasValue)
                {
                    TempData["ErrorMessage"] = "Thông tin tài khoản của bạn không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra dữ liệu để debug
                if (cuuSinhVien.SinhVien == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên liên quan.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chuyên ngành của sinh viên.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa == null)
                {
                    TempData["ErrorMessage"] = "Chuyên ngành không có mã khoa.";
                    return RedirectToAction(nameof(Index));
                }
                if (currentUser.MaKhoa != cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa)
                {
                    TempData["ErrorMessage"] = $"Bạn không có quyền xem thông tin cựu sinh viên này. (Mã khoa của bạn: {currentUser.MaKhoa}, Mã khoa của sinh viên: {cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa})";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(cuuSinhVien);
        }

        [Authorize(Roles = "Admin,Khoa")]
        public async Task<IActionResult> Edit(string mssv)
        {
            if (string.IsNullOrEmpty(mssv)) return NotFound();

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(mssv);
            if (cuuSinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || !currentUser.MaKhoa.HasValue)
                {
                    TempData["ErrorMessage"] = "Thông tin tài khoản của bạn không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (cuuSinhVien.SinhVien == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên liên quan.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chuyên ngành của sinh viên.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa == null)
                {
                    TempData["ErrorMessage"] = "Chuyên ngành không có mã khoa.";
                    return RedirectToAction(nameof(Index));
                }
                if (currentUser.MaKhoa != cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa)
                {
                    TempData["ErrorMessage"] = $"Bạn không có quyền chỉnh sửa thông tin cựu sinh viên này. (Mã khoa của bạn: {currentUser.MaKhoa}, Mã khoa của sinh viên: {cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa})";
                    return RedirectToAction(nameof(Index));
                }
            }

            var model = new ManageCuuSinhVienViewModel
            {
                NamTotNghiep = cuuSinhVien.NamTotNghiep,
                XepLoaiTotNghiep = cuuSinhVien.XepLoaiTotNghiep,
                DiaChiHienTai = cuuSinhVien.DiaChiHienTai,
                HinhThucLienLac = cuuSinhVien.HinhThucLienLac
            };

            ViewBag.MSSV = mssv;
            return View(model);
        }

        [Authorize(Roles = "Admin,Khoa")]
        public async Task<IActionResult> Delete(string mssv)
        {
            if (string.IsNullOrEmpty(mssv)) return NotFound();

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(mssv);
            if (cuuSinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || !currentUser.MaKhoa.HasValue)
                {
                    TempData["ErrorMessage"] = "Thông tin tài khoản của bạn không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (cuuSinhVien.SinhVien == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên liên quan.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chuyên ngành của sinh viên.";
                    return RedirectToAction(nameof(Index));
                }
                if (cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa == null)
                {
                    TempData["ErrorMessage"] = "Chuyên ngành không có mã khoa.";
                    return RedirectToAction(nameof(Index));
                }
                if (currentUser.MaKhoa != cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa)
                {
                    TempData["ErrorMessage"] = $"Bạn không có quyền xóa thông tin cựu sinh viên này. (Mã khoa của bạn: {currentUser.MaKhoa}, Mã khoa của sinh viên: {cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa})";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(cuuSinhVien);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Khoa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string mssv, ManageCuuSinhVienViewModel model)
        {
            if (string.IsNullOrEmpty(mssv)) return NotFound();

            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(mssv);
            if (cuuSinhVien == null) return NotFound();

            if (User.IsInRole("Khoa"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.MaKhoa == null || cuuSinhVien.SinhVien?.ChuyenNganh?.MaKhoa == null || currentUser.MaKhoa != cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa thông tin cựu sinh viên này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (ModelState.IsValid)
            {
                cuuSinhVien.NamTotNghiep = model.NamTotNghiep;
                cuuSinhVien.XepLoaiTotNghiep = model.XepLoaiTotNghiep;
                cuuSinhVien.DiaChiHienTai = model.DiaChiHienTai;
                cuuSinhVien.HinhThucLienLac = model.HinhThucLienLac;

                await _cuuSinhVienRepository.UpdateAsync(cuuSinhVien);
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MSSV = mssv; // Truyền lại MSSV nếu có lỗi
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Khoa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string mssv)
        {
            var cuuSinhVien = await _cuuSinhVienRepository.GetByMssvAsync(mssv);
            if (cuuSinhVien == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Khoa"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.MaKhoa == null || cuuSinhVien.SinhVien?.ChuyenNganh?.MaKhoa == null || currentUser.MaKhoa != cuuSinhVien.SinhVien.ChuyenNganh.MaKhoa)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xóa thông tin cựu sinh viên này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            try
            {
                // Xóa các công việc liên quan
                var congViecs = await _congViecRepository.GetByMssvAsync(mssv);
                foreach (var congViec in congViecs)
                {
                    await _congViecRepository.DeleteAsync(congViec.MaCongViec);
                }

                // Xóa các thành tựu liên quan
                var thanhTuus = await _thanhTuuRepository.GetByMssvAsync(mssv);
                foreach (var thanhTuu in thanhTuus)
                {
                    await _thanhTuuRepository.DeleteAsync(thanhTuu.MaThanhTuu);
                }

                // Đặt lại trạng thái SinhVien
                var sinhVien = await _sinhVienRepository.GetByIdAsync(mssv);
                if (sinhVien != null)
                {
                    sinhVien.TrangThai = TrangThaiSinhVien.ConHoc;
                    try
                    {
                        await _sinhVienRepository.UpdateAsync(sinhVien);
                    }
                    catch (DbUpdateException ex)
                    {
                        // Xử lý lỗi nếu có ràng buộc hoặc cập nhật thất bại
                        throw new Exception($"Lỗi khi cập nhật trạng thái SinhVien: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }

                // Xóa tài khoản người dùng (NguoiDung)
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.MSSV == mssv);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Không thể xóa tài khoản người dùng liên kết: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Xóa thông tin cựu sinh viên
                await _cuuSinhVienRepository.DeleteAsync(mssv);
                TempData["SuccessMessage"] = "Xóa cựu sinh viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa cựu sinh viên: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}