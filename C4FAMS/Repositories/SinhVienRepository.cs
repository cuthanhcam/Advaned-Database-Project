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
            _context = context;
        }

        public async Task<IEnumerable<SinhVien>> GetAllAsync()
        {
            return await _context.SinhVien.Include(s => s.Khoa).ToListAsync();
        }

        public async Task<SinhVien?> GetByIdAsync(string mssv)
        {
            return await _context.SinhVien.Include(s => s.Khoa).FirstOrDefaultAsync(s => s.MSSV == mssv);
        }

        public async Task AddAsync(SinhVien sinhVien)
        {
            _context.SinhVien.Add(sinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SinhVien sinhVien)
        {
            _context.SinhVien.Update(sinhVien);
            await _context.SaveChangesAsync();
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

        public async Task<IEnumerable<IGrouping<string, SinhVien>>> GetGroupedByKhoaAsync()
        {
            return await _context.SinhVien
                .Include(s => s.Khoa)
                .GroupBy(s => s.Khoa.TenKhoa)
                .ToListAsync();
        }
    }
}