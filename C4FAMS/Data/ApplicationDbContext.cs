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

        // Các DbSet hiện có
        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<ChuyenNganh> ChuyenNganh { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<SuKien> SuKiens { get; set; }
        public DbSet<SuKienChiTiet> SuKienChiTiets { get; set; }
        public DbSet<SuKienHinhAnh> SuKienHinhAnhs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Định nghĩa khóa chính cho các bảng hiện có
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

            // Định nghĩa khóa chính cho các bảng sự kiện
            builder.Entity<SuKien>().HasKey(s => s.MaSuKien);
            builder.Entity<SuKienChiTiet>().HasKey(sc => sc.MaSuKien);
            builder.Entity<SuKienHinhAnh>().HasKey(sh => sh.MaHinhAnh);

            // Quan hệ giữa SuKien và Khoa (1-nhiều)
            builder.Entity<SuKien>()
                .HasOne(s => s.Khoa)
                .WithMany(k => k.SuKiens) // Bây giờ không lỗi vì SuKiens đã được khai báo
                .HasForeignKey(s => s.MaKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ giữa SuKien và SuKienChiTiet (1-1)
            builder.Entity<SuKienChiTiet>()
                .HasOne(sc => sc.SuKien)
                .WithOne()
                .HasForeignKey<SuKienChiTiet>(sc => sc.MaSuKien)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ giữa SuKien và SuKienHinhAnh (1-nhiều)
            builder.Entity<SuKienHinhAnh>()
                .HasOne(sh => sh.SuKien)
                .WithMany(s => s.SuKienHinhAnhs)
                .HasForeignKey(sh => sh.MaSuKien)
                .OnDelete(DeleteBehavior.Cascade);

            // Gọi SeedData (nếu có)
            SeedData.Seed(builder);
        }
    }
}