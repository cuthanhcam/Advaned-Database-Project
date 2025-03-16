using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class SinhVienRepository : ISinhVienRepository
    {
        private readonly ApplicationDbContext _context;

        public SinhVienRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<SinhVien>> GetAllAsync()
        {
            return await _context.SinhVien
                .Include(s => s.ChuyenNganh)
                .ThenInclude(c => c.Khoa)
                .ToListAsync();
        }

        public async Task<IEnumerable<SinhVien>> GetByKhoaAsync(int maKhoa)
        {
            return await _context.SinhVien
                .Include(s => s.ChuyenNganh)
                .ThenInclude(c => c.Khoa)
                .Where(s => s.ChuyenNganh != null && s.ChuyenNganh.MaKhoa == maKhoa)
                .ToListAsync();
        }

        public async Task<SinhVien?> GetByIdAsync(string mssv)
        {
            return await _context.SinhVien
                .Include(s => s.ChuyenNganh)
                .ThenInclude(c => c.Khoa)
                .FirstOrDefaultAsync(s => s.MSSV == mssv);
        }

        public async Task AddAsync(SinhVien sinhVien)
        {
            if (sinhVien == null) throw new ArgumentNullException(nameof(sinhVien));
            _context.SinhVien.Add(sinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SinhVien sinhVien)
        {
            if (sinhVien == null) throw new ArgumentNullException(nameof(sinhVien));
            var existingSinhVien = await _context.SinhVien.FindAsync(sinhVien.MSSV);
            if (existingSinhVien != null)
            {
                _context.Entry(existingSinhVien).CurrentValues.SetValues(sinhVien);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string mssv)
        {
            var sinhVien = await _context.SinhVien.FindAsync(mssv);
            if (sinhVien != null)
            {
                _context.SinhVien.Remove(sinhVien);
                await _context.SaveChangesAsync();
            }
        }
    }
}