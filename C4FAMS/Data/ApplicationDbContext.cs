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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Khoa>().HasKey(k => k.MaKhoa);
            builder.Entity<ChuyenNganh>().HasKey(c => c.MaChuyenNganh);
            builder.Entity<SinhVien>().HasKey(s => s.MSSV);

            builder.Entity<ChuyenNganh>()
                .HasOne(c => c.Khoa)
                .WithMany(k => k.ChuyenNganh)
                .HasForeignKey(c => c.MaKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SinhVien>()
                .HasOne(s => s.ChuyenNganh)
                .WithMany()
                .HasForeignKey(s => s.MaChuyenNganh)
                .OnDelete(DeleteBehavior.Restrict);

            SeedData.Seed(builder);
        }
    }
}