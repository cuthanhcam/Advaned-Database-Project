using C4FAMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<NguoiDung, IdentityRole<int>, int>
    {
        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<CuuSinhVien> CuuSinhVien { get; set; }
        public DbSet<SuKien> SuKien { get; set; }
        public DbSet<DangKySuKien> DangKySuKien { get; set; }
        public DbSet<CongViec> CongViec { get; set; }
        public DbSet<ThanhTuu> ThanhTuu { get; set; }
        public DbSet<ThongBao> ThongBao { get; set; }
        public DbSet<ChuyenNganh> ChuyenNganh { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint for DangKySuKien
            modelBuilder.Entity<DangKySuKien>()
                .HasIndex(d => new { d.MSSV, d.MaSuKien })
                .IsUnique();

            // Configure relationships for DangKySuKien
            modelBuilder.Entity<DangKySuKien>()
                .HasOne(d => d.CuuSinhVien)
                .WithMany()
                .HasForeignKey(d => d.MSSV)
                .OnDelete(DeleteBehavior.Cascade); // Giữ CASCADE cho MSSV

            modelBuilder.Entity<DangKySuKien>()
                .HasOne(d => d.SuKien)
                .WithMany()
                .HasForeignKey(d => d.MaSuKien)
                .OnDelete(DeleteBehavior.NoAction); // Chuyển sang NO ACTION cho MaSuKien

            modelBuilder.Entity<ChuyenNganh>()
                .HasIndex(c => c.TenChuyenNganh)
                .IsUnique();
        }
    }
}