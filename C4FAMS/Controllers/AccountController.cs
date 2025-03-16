using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace C4FAMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly IKhoaRepository _khoaRepository;

        public AccountController(
            UserManager<NguoiDung> userManager,
            SignInManager<NguoiDung> signInManager,
            ISinhVienRepository sinhVienRepository,
            IKhoaRepository khoaRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sinhVienRepository = sinhVienRepository;
            _khoaRepository = khoaRepository;
        }

        // GET: Account/Register (Đăng ký Cựu Sinh Viên)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Account/Register (Đăng ký Cựu Sinh Viên)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string mssv)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra MSSV
                var sinhVien = await _sinhVienRepository.GetByIdAsync(mssv);
                if (sinhVien == null || sinhVien.TrangThai != TrangThaiSinhVien.DaTotNghiep)
                {
                    ModelState.AddModelError("MSSV", "MSSV không hợp lệ hoặc sinh viên chưa tốt nghiệp.");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Tạo tài khoản Cựu Sinh Viên
                var user = new NguoiDung
                {
                    UserName = model.Email, // Sử dụng Email làm UserName
                    Email = model.Email,
                    VaiTro = "CuuSinhVien",
                    TrangThai = true,
                    MaKhoa = sinhVien.ChuyenNganh?.Khoa?.MaKhoa, // Lấy MaKhoa từ ChuyenNganh.Khoa
                    MSSV = mssv,
                    SinhVien = sinhVien
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "CuuSinhVien");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                if (!user.TrangThai)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Manage
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ManageViewModel
            {
                Email = user.Email,
                VaiTro = user.VaiTro,
                TrangThai = user.TrangThai,
                TenKhoa = user.Khoa?.TenKhoa
            };

            return View(model);
        }

        // POST: Account/Manage (Change Password)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                if (ModelState.IsValid)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (changePasswordResult.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
                        return RedirectToAction(nameof(Manage));
                    }

                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            model.Email = user.Email;
            model.VaiTro = user.VaiTro;
            model.TrangThai = user.TrangThai;
            model.TenKhoa = user.Khoa?.TenKhoa;

            return View(model);
        }

        // GET: Account/ManageUsers (Quản lý tài khoản - Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .Where(u => u.VaiTro == "Khoa" || u.VaiTro == "Admin") // Chỉ hiển thị Khoa và Admin
                .Include(u => u.Khoa)
                .ToListAsync();
            return View(users);
        }

        // GET: Account/CreateUser (Tạo tài khoản Khoa hoặc Admin - Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(new RegisterViewModel());
        }

        // POST: Account/CreateUser (Tạo tài khoản Khoa hoặc Admin - Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(RegisterViewModel model, string vaiTro, int maKhoa)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                    return View(model);
                }

                // Kiểm tra MaKhoa hợp lệ
                var khoa = await _khoaRepository.GetByIdAsync(maKhoa);
                if (khoa == null)
                {
                    ModelState.AddModelError("MaKhoa", "Mã khoa không tồn tại.");
                    ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                    return View(model);
                }

                // Vai trò phải là Khoa hoặc Admin
                if (vaiTro != "Khoa" && vaiTro != "Admin")
                {
                    ModelState.AddModelError("VaiTro", "Vai trò phải là Khoa hoặc Admin.");
                    ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                    return View(model);
                }

                // Tạo tài khoản
                var user = new NguoiDung
                {
                    UserName = model.Email,
                    Email = model.Email,
                    VaiTro = vaiTro, // Vai trò được chọn từ form (Khoa hoặc Admin)
                    TrangThai = true,
                    MaKhoa = maKhoa
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, vaiTro);
                    return RedirectToAction(nameof(ManageUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(model);
        }

        // GET: Account/EditUser (Chỉnh sửa tài khoản - Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.Users
                .Include(u => u.Khoa)
                .FirstOrDefaultAsync(u => u.Id == id && (u.VaiTro == "Khoa" || u.VaiTro == "Admin"));

            if (user == null) return NotFound();

            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(user);
        }

        // POST: Account/EditUser (Chỉnh sửa tài khoản - Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, NguoiDung nguoiDung)
        {
            if (id != nguoiDung.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(id.ToString());
                if (existingUser == null || (existingUser.VaiTro != "Khoa" && existingUser.VaiTro != "Admin")) return NotFound();

                existingUser.UserName = nguoiDung.UserName;
                existingUser.Email = nguoiDung.Email;
                existingUser.MaKhoa = nguoiDung.MaKhoa;
                existingUser.TrangThai = nguoiDung.TrangThai;

                if (existingUser.MaKhoa.HasValue)
                {
                    var khoa = await _khoaRepository.GetByIdAsync(existingUser.MaKhoa.Value);
                    if (khoa == null)
                    {
                        ModelState.AddModelError("MaKhoa", "Mã khoa không tồn tại.");
                        ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
                        return View(nguoiDung);
                    }
                }

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ManageUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.KhoaList = await _khoaRepository.GetAllAsync();
            return View(nguoiDung);
        }

        // GET: Account/DeleteUser (Xóa tài khoản - Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.Users
                .Include(u => u.Khoa)
                .FirstOrDefaultAsync(u => u.Id == id && (u.VaiTro == "Khoa" || u.VaiTro == "Admin"));

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Account/DeleteUser (Xác nhận xóa tài khoản - Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || (user.VaiTro != "Khoa" && user.VaiTro != "Admin")) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ManageUsers));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(user);
        }
    }
}