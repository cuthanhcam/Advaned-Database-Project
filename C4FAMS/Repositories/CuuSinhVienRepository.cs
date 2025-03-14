using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class CuuSinhVienRepository : ICuuSinhVienRepository
    {
        private readonly ApplicationDbContext _context;

        public CuuSinhVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CuuSinhVien>> GetAllAsync()
        {
            return await _context.CuuSinhVien.Include(c => c.SinhVien).ThenInclude(s => s.Khoa).ToListAsync();
        }

        public async Task<CuuSinhVien?> GetByIdAsync(string mssv)
        {
            return await _context.CuuSinhVien.Include(c => c.SinhVien).ThenInclude(s => s.Khoa).FirstOrDefaultAsync(c => c.MSSV == mssv);
        }

        public async Task AddAsync(CuuSinhVien cuuSinhVien)
        {
            _context.CuuSinhVien.Add(cuuSinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CuuSinhVien cuuSinhVien)
        {
            _context.CuuSinhVien.Update(cuuSinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string mssv)
        {
            var cuuSinhVien = await _context.CuuSinhVien.FindAsync(mssv);
            if (cuuSinhVien != null)
            {
                _context.CuuSinhVien.Remove(cuuSinhVien);
                await _context.SaveChangesAsync();
            }
        }
    }
}