using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.DangKySuKien
                .Include(d => d.CuuSinhVien).ThenInclude(c => c.SinhVien)
                .Include(d => d.SuKien)
                .ToListAsync();
        }

        public async Task<DangKySuKien?> GetByIdAsync(int id)
        {
            return await _context.DangKySuKien
                .Include(d => d.CuuSinhVien).ThenInclude(c => c.SinhVien)
                .Include(d => d.SuKien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);
        }

        public async Task AddAsync(DangKySuKien dangKySuKien)
        {
            _context.DangKySuKien.Add(dangKySuKien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dangKySuKien = await _context.DangKySuKien.FindAsync(id);
            if (dangKySuKien != null)
            {
                _context.DangKySuKien.Remove(dangKySuKien);
                await _context.SaveChangesAsync();
            }
        }
    }
}