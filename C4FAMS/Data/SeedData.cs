using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using C4FAMS.Models;

namespace C4FAMS.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder builder)
        {
            // Seed Roles
            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "Khoa", NormalizedName = "KHOA" },
                new IdentityRole<int> { Id = 3, Name = "CuuSinhVien", NormalizedName = "CUUSINHVIEN" }
            );

            // Tạo PasswordHasher để mã hóa mật khẩu
            var hasher = new PasswordHasher<NguoiDung>();

            // Seed Users với SecurityStamp
            builder.Entity<NguoiDung>().HasData(
                new NguoiDung
                {
                    Id = 1,
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    VaiTro = "Admin",
                    TrangThai = true
                },
                new NguoiDung
                {
                    Id = 2,
                    UserName = "khoa1@example.com",
                    NormalizedUserName = "KHOA1@EXAMPLE.COM",
                    Email = "khoa1@example.com",
                    NormalizedEmail = "KHOA1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Khoa@123"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    VaiTro = "Khoa",
                    MaKhoa = 1,
                    TrangThai = true
                },
                new NguoiDung
                {
                    Id = 3,
                    UserName = "cuusv@example.com",
                    NormalizedUserName = "CUUSV@EXAMPLE.COM",
                    Email = "cuusv@example.com",
                    NormalizedEmail = "CUUSV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "CuuSV@123"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    VaiTro = "CuuSinhVien",
                    TrangThai = true
                }
            );

            // Seed UserRoles
            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 },
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 },
                new IdentityUserRole<int> { UserId = 3, RoleId = 3 }
            );
        }

        public static async Task SeedInitialData(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Starting data seeding...");

                // Seed Khoa
                if (!context.Khoa.Any())
                {
                    logger.LogInformation("Seeding Khoa...");
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Bật IDENTITY_INSERT
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Khoa] ON");

                            context.Khoa.AddRange(
                                new Khoa { MaKhoa = 1, TenKhoa = "Công Nghệ Thông Tin", Email = "cntt@hutech.edu.vn", SoDienThoai = "02854450000" },
                                new Khoa { MaKhoa = 2, TenKhoa = "Ngoại Ngữ", Email = "nn@hutech.edu.vn", SoDienThoai = "02854450001" },
                                new Khoa { MaKhoa = 3, TenKhoa = "Cơ Khí", Email = "ck@hutech.edu.vn", SoDienThoai = "02854450002" }
                            );
                            await context.SaveChangesAsync();

                            // Tắt IDENTITY_INSERT
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Khoa] OFF");

                            await transaction.CommitAsync();
                            logger.LogInformation("Seeded Khoa successfully.");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            logger.LogError(ex, "Error seeding Khoa.");
                            throw;
                        }
                    }
                }

                // Seed ChuyenNganh
                if (!context.ChuyenNganh.Any())
                {
                    logger.LogInformation("Seeding ChuyenNganh...");
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Bật IDENTITY_INSERT
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ChuyenNganh] ON");

                            context.ChuyenNganh.AddRange(
                                new ChuyenNganh { MaChuyenNganh = 1, TenChuyenNganh = "Công Nghệ Phần Mềm", MaKhoa = 1 },
                                new ChuyenNganh { MaChuyenNganh = 2, TenChuyenNganh = "Hệ Thống Thông Tin", MaKhoa = 1 },
                                new ChuyenNganh { MaChuyenNganh = 3, TenChuyenNganh = "Trí Tuệ Nhân Tạo", MaKhoa = 1 },
                                new ChuyenNganh { MaChuyenNganh = 4, TenChuyenNganh = "Ngôn Ngữ Anh", MaKhoa = 2 },
                                new ChuyenNganh { MaChuyenNganh = 5, TenChuyenNganh = "Ngôn Ngữ Trung", MaKhoa = 2 },
                                new ChuyenNganh { MaChuyenNganh = 6, TenChuyenNganh = "Ngôn Ngữ Hàn", MaKhoa = 2 },
                                new ChuyenNganh { MaChuyenNganh = 7, TenChuyenNganh = "Cơ Khí Chế Tạo", MaKhoa = 3 },
                                new ChuyenNganh { MaChuyenNganh = 8, TenChuyenNganh = "Cơ Điện Tử", MaKhoa = 3 },
                                new ChuyenNganh { MaChuyenNganh = 9, TenChuyenNganh = "Kỹ Thuật Ô Tô", MaKhoa = 3 }
                            );
                            await context.SaveChangesAsync();

                            // Tắt IDENTITY_INSERT
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ChuyenNganh] OFF");

                            await transaction.CommitAsync();
                            logger.LogInformation("Seeded ChuyenNganh successfully.");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            logger.LogError(ex, "Error seeding ChuyenNganh.");
                            throw;
                        }
                    }
                }

                logger.LogInformation("Data seeding completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding initial data.");
                throw;
            }
        }
    }
}