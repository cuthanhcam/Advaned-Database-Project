using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class SuKienChiTietRepository : ISuKienChiTietRepository
    {
        private readonly ApplicationDbContext _context;

        public SuKienChiTietRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SuKienChiTiet> GetByIdAsync(int maSuKien)
        {
            return await _context.SuKienChiTiets
                .Include(s => s.SuKien)
                .FirstOrDefaultAsync(s => s.MaSuKien == maSuKien);
        }

        public async Task AddAsync(SuKienChiTiet suKienChiTiet)
        {
            _context.SuKienChiTiets.Add(suKienChiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuKienChiTiet suKienChiTiet)
        {
            _context.SuKienChiTiets.Update(suKienChiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maSuKien)
        {
            var suKienChiTiet = await GetByIdAsync(maSuKien);
            if (suKienChiTiet != null)
            {
                _context.SuKienChiTiets.Remove(suKienChiTiet);
                await _context.SaveChangesAsync();
            }
        }
    }
}