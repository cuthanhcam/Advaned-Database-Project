using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class SuKienHinhAnhRepository : ISuKienHinhAnhRepository
    {
        private readonly ApplicationDbContext _context;

        public SuKienHinhAnhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SuKienHinhAnh>> GetBySuKienAsync(int maSuKien)
        {
            return await _context.SuKienHinhAnhs
                .Where(h => h.MaSuKien == maSuKien)
                .ToListAsync();
        }

        public async Task<SuKienHinhAnh> GetByIdAsync(int maHinhAnh)
        {
            return await _context.SuKienHinhAnhs
                .FirstOrDefaultAsync(h => h.MaHinhAnh == maHinhAnh);
        }

        public async Task AddAsync(SuKienHinhAnh suKienHinhAnh)
        {
            _context.SuKienHinhAnhs.Add(suKienHinhAnh);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maHinhAnh)
        {
            var hinhAnh = await GetByIdAsync(maHinhAnh);
            if (hinhAnh != null)
            {
                _context.SuKienHinhAnhs.Remove(hinhAnh);
                await _context.SaveChangesAsync();
            }
        }
    }
}