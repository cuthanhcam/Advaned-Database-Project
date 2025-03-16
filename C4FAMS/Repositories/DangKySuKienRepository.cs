using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Repositories
{
    public class DangKySuKienRepository : IDangKySuKienRepository
    {
        private readonly ApplicationDbContext _context;

        public DangKySuKienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DangKySuKien>> GetAllAsync()
        {
            return await _context.DangKySuKiens
                .Include(d => d.SuKien)
                .Include(d => d.CuuSinhVien)
                .ToListAsync();
        }

        public async Task<DangKySuKien> GetByIdAsync(int id)
        {
            return await _context.DangKySuKiens
                .Include(d => d.SuKien)
                .Include(d => d.CuuSinhVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);
        }

        public async Task<IEnumerable<DangKySuKien>> GetBySuKienAsync(int maSuKien)
        {
            return await _context.DangKySuKiens
                .Include(d => d.SuKien)
                .Include(d => d.CuuSinhVien)
                    .ThenInclude(c => c.SinhVien) // Include SinhVien để lấy HoTen
                .Where(d => d.MaSuKien == maSuKien)
                .ToListAsync();
        }

        public async Task<IEnumerable<DangKySuKien>> GetByCuuSinhVienAsync(string mssv)
        {
            return await _context.DangKySuKiens
                .Include(d => d.SuKien)
                .Include(d => d.CuuSinhVien)
                .Where(d => d.MSSV == mssv)
                .ToListAsync();
        }

        public async Task AddAsync(DangKySuKien dangKy)
        {
            _context.DangKySuKiens.Add(dangKy);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DangKySuKien dangKy)
        {
            _context.DangKySuKiens.Update(dangKy);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dangKy = await _context.DangKySuKiens.FindAsync(id);
            if (dangKy != null)
            {
                _context.DangKySuKiens.Remove(dangKy);
                await _context.SaveChangesAsync();
            }
        }
    }
}