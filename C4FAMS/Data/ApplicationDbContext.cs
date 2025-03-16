// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using C4FAMS.Models;
using Microsoft.AspNetCore.Identity;

namespace C4FAMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<NguoiDung, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<ChuyenNganh> ChuyenNganh { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<SuKien> SuKiens { get; set; }
        public DbSet<SuKienChiTiet> SuKienChiTiets { get; set; }
        public DbSet<SuKienHinhAnh> SuKienHinhAnhs { get; set; }
        public DbSet<SuKienSinhVien> SuKienSinhViens { get; set; }
        public DbSet<CuuSinhVien> CuuSinhVien { get; set; }
        public DbSet<CongViec> CongViec { get; set; }
        public DbSet<ThanhTuu> ThanhTuu { get; set; }
        public DbSet<DangKySuKien> DangKySuKiens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Khoa>().HasKey(k => k.MaKhoa);
            builder.Entity<ChuyenNganh>().HasKey(c => c.MaChuyenNganh);
            builder.Entity<SinhVien>().HasKey(s => s.MSSV);

            // Quan hệ giữa Khoa và ChuyenNganh (1-nhiều)
            builder.Entity<ChuyenNganh>()
                .HasOne(c => c.Khoa)
                .WithMany(k => k.ChuyenNganh)
                .HasForeignKey(c => c.MaKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa SinhVien và ChuyenNganh (1-nhiều)
            builder.Entity<SinhVien>()
                .HasOne(s => s.ChuyenNganh)
                .WithMany()
                .HasForeignKey(s => s.MaChuyenNganh)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa NguoiDung và Khoa (1-nhiều)
            builder.Entity<NguoiDung>()
                .HasOne(u => u.Khoa)
                .WithMany()
                .HasForeignKey(u => u.MaKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa SuKien và Khoa
            builder.Entity<SuKien>()
                .HasOne(s => s.Khoa)
                .WithMany(k => k.SuKiens)
                .HasForeignKey(s => s.MaKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ nhiều-nhiều giữa SuKien và SinhVien qua SuKienSinhVien
            builder.Entity<SuKienSinhVien>()
                .HasKey(ss => new { ss.MaSuKien, ss.MSSV });

            builder.Entity<SuKienSinhVien>()
                .HasOne(ss => ss.SuKien)
                .WithMany(s => s.SuKienSinhViens)
                .HasForeignKey(ss => ss.MaSuKien);

            builder.Entity<SuKienSinhVien>()
                .HasOne(ss => ss.SinhVien)
                .WithMany(s => s.SuKienSinhViens)
                .HasForeignKey(ss => ss.MSSV);

            // Định nghĩa các khóa chính khác
            builder.Entity<SuKien>().HasKey(s => s.MaSuKien);
            builder.Entity<SuKienChiTiet>().HasKey(sc => sc.MaSuKien);
            builder.Entity<SuKienHinhAnh>().HasKey(sh => sh.MaHinhAnh);

            // Quan hệ giữa SuKien và SuKienChiTiet
            builder.Entity<SuKienChiTiet>()
                .HasOne(sc => sc.SuKien)
                .WithOne()
                .HasForeignKey<SuKienChiTiet>(sc => sc.MaSuKien)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ giữa SuKien và SuKienHinhAnh
            builder.Entity<SuKienHinhAnh>()
                .HasOne(sh => sh.SuKien)
                .WithMany(s => s.SuKienHinhAnhs)
                .HasForeignKey(sh => sh.MaSuKien)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ giữa NguoiDung và CuuSinhVien (1-1)
            builder.Entity<NguoiDung>()
                .HasOne(u => u.CuuSinhVien)
                .WithOne()
                .HasForeignKey<NguoiDung>(u => u.MSSV)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa CuuSinhVien và SinhVien (1-1)
            builder.Entity<CuuSinhVien>()
                .HasOne(c => c.SinhVien)
                .WithOne()
                .HasForeignKey<CuuSinhVien>(c => c.MSSV)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa CuuSinhVien và CongViec (1-nhiều)
            builder.Entity<CongViec>()
                .HasOne(c => c.CuuSinhVien)
                .WithMany(c => c.CongViec) // Đã có thuộc tính `CongViec` trong `CuuSinhVien`
                .HasForeignKey(c => c.MSSV)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ giữa CuuSinhVien và ThanhTuu (1-nhiều)
            builder.Entity<ThanhTuu>()
                .HasOne(t => t.CuuSinhVien)
                .WithMany(t => t.ThanhTuu) // Đã có thuộc tính `ThanhTuu` trong `CuuSinhVien`
                .HasForeignKey(t => t.MSSV)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData.Seed(builder);
        }
    }
}