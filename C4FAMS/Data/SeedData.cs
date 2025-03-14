using C4FAMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<NguoiDung>>();

            // Tạo các vai trò
            string[] roleNames = { "Admin", "Khoa", "CuuSinhVien" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            // Seed tài khoản Admin
            var adminUser = new NguoiDung
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                VaiTro = "Admin",
                TrangThai = true
            };
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed tài khoản Khoa
            var khoaUser = new NguoiDung
            {
                UserName = "khoa@example.com",
                Email = "khoa@example.com",
                VaiTro = "Khoa",
                TrangThai = true
            };
            if (await userManager.FindByEmailAsync(khoaUser.Email) == null)
            {
                await userManager.CreateAsync(khoaUser, "Khoa@123");
                await userManager.AddToRoleAsync(khoaUser, "Khoa");
            }

            // Seed tài khoản CuuSinhVien
            var cuuSinhVienUser = new NguoiDung
            {
                UserName = "cuusinhvien@example.com",
                Email = "cuusinhvien@example.com",
                VaiTro = "CuuSinhVien",
                TrangThai = true
            };
            if (await userManager.FindByEmailAsync(cuuSinhVienUser.Email) == null)
            {
                await userManager.CreateAsync(cuuSinhVienUser, "CuuSV@123");
                await userManager.AddToRoleAsync(cuuSinhVienUser, "CuuSinhVien");
            }

            // Seed dữ liệu Khoa
            if (!context.Khoa.Any())
            {
                context.Khoa.AddRange(
                    new Khoa { TenKhoa = "CNTT", Email = "cntt@example.com", SoDienThoai = "0123456789" },
                    new Khoa { TenKhoa = "Kinh Tế", Email = "kinhte@example.com", SoDienThoai = "0987654321" },
                    new Khoa { TenKhoa = "Cơ Khí", Email = "cokhi@example.com", SoDienThoai = "0912345678" }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu SinhVien
            if (!context.SinhVien.Any())
            {
                context.SinhVien.AddRange(
                    new SinhVien { MSSV = "SV001", HoTen = "Nguyễn Văn A", GioiTinh = true, NgaySinh = new DateTime(2000, 1, 1), QueQuan = "Hà Nội", NganhHoc = "CNTT", MaKhoa = 1, Email = "a@example.com", SoDienThoai = "0911111111" },
                    new SinhVien { MSSV = "SV002", HoTen = "Trần Thị B", GioiTinh = false, NgaySinh = new DateTime(1999, 5, 10), QueQuan = "TP.HCM", NganhHoc = "Kinh Tế", MaKhoa = 2, Email = "b@example.com", SoDienThoai = "0922222222" },
                    new SinhVien { MSSV = "SV003", HoTen = "Lê Văn C", GioiTinh = true, NgaySinh = new DateTime(2001, 3, 15), QueQuan = "Đà Nẵng", NganhHoc = "Cơ Khí", MaKhoa = 3, Email = "c@example.com", SoDienThoai = "0933333333" },
                    new SinhVien { MSSV = "SV004", HoTen = "Phạm Thị D", GioiTinh = false, NgaySinh = new DateTime(2000, 7, 20), QueQuan = "Huế", NganhHoc = "CNTT", MaKhoa = 1, Email = "d@example.com", SoDienThoai = "0944444444" }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu CuuSinhVien
            if (!context.CuuSinhVien.Any())
            {
                context.CuuSinhVien.AddRange(
                    new CuuSinhVien { MSSV = "SV001", NamTotNghiep = 2022, XepLoaiTotNghiep = "Giỏi", DiaChiHienTai = "Hà Nội", HinhThucLienLac = "Email" },
                    new CuuSinhVien { MSSV = "SV002", NamTotNghiep = 2021, XepLoaiTotNghiep = "Khá", DiaChiHienTai = "TP.HCM", HinhThucLienLac = "Phone" },
                    new CuuSinhVien { MSSV = "SV003", NamTotNghiep = 2023, XepLoaiTotNghiep = "Xuất sắc", DiaChiHienTai = "Đà Nẵng", HinhThucLienLac = "Email" }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu SuKien
            if (!context.SuKien.Any())
            {
                context.SuKien.AddRange(
                    new SuKien { TenSuKien = "Hội thảo CNTT", NgayToChuc = new DateTime(2025, 4, 1, 9, 0, 0), DiaDiem = "Hội trường A", MaKhoa = 1 },
                    new SuKien { TenSuKien = "Gặp mặt cựu SV Kinh Tế", NgayToChuc = new DateTime(2025, 5, 15, 14, 0, 0), DiaDiem = "Hội trường B", MaKhoa = 2 },
                    new SuKien { TenSuKien = "Triển lãm Cơ Khí", NgayToChuc = new DateTime(2025, 6, 20, 10, 0, 0), DiaDiem = "Hội trường C", MaKhoa = 3 }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu DangKySuKien
            if (!context.DangKySuKien.Any())
            {
                context.DangKySuKien.AddRange(
                    new DangKySuKien { MSSV = "SV001", MaSuKien = 1, NgayDangKy = DateTime.Now.AddDays(-5) },
                    new DangKySuKien { MSSV = "SV002", MaSuKien = 2, NgayDangKy = DateTime.Now.AddDays(-3) },
                    new DangKySuKien { MSSV = "SV003", MaSuKien = 3, NgayDangKy = DateTime.Now.AddDays(-1) }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu CongViec
            if (!context.CongViec.Any())
            {
                context.CongViec.AddRange(
                    new CongViec { MSSV = "SV001", ViTri = "Lập trình viên", CongTy = "FPT", LinhVuc = "CNTT", ThuNhap = 15000000 },
                    new CongViec { MSSV = "SV002", ViTri = "Nhân viên kinh doanh", CongTy = "VinGroup", LinhVuc = "Kinh Tế", ThuNhap = 12000000 },
                    new CongViec { MSSV = "SV003", ViTri = "Kỹ sư cơ khí", CongTy = "Toyota", LinhVuc = "Cơ Khí", ThuNhap = 18000000 }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu ThanhTuu
            if (!context.ThanhTuu.Any())
            {
                context.ThanhTuu.AddRange(
                    new ThanhTuu { MSSV = "SV001", TenThanhTuu = "Giải nhất lập trình", MoTa = "Cuộc thi ACM 2022", NamDatDuoc = 2022 },
                    new ThanhTuu { MSSV = "SV002", TenThanhTuu = "Top 10 kinh doanh trẻ", MoTa = "Diễn đàn Kinh Tế 2021", NamDatDuoc = 2021 },
                    new ThanhTuu { MSSV = "SV003", TenThanhTuu = "Bằng sáng chế cơ khí", MoTa = "Sáng chế máy mới", NamDatDuoc = 2023 }
                );
                await context.SaveChangesAsync();
            }

            // Seed dữ liệu ThongBao
            if (!context.ThongBao.Any())
            {
                context.ThongBao.AddRange(
                    new ThongBao { MSSV = "SV001", NoiDung = "Mời tham gia hội thảo CNTT", NgayGui = DateTime.Now.AddDays(-7), TrangThai = false },
                    new ThongBao { MSSV = "SV002", NoiDung = "Gặp mặt cựu SV Kinh Tế", NgayGui = DateTime.Now.AddDays(-5), TrangThai = true },
                    new ThongBao { MSSV = "SV003", NoiDung = "Thông báo triển lãm Cơ Khí", NgayGui = DateTime.Now.AddDays(-3), TrangThai = false }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}