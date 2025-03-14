using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class SuKienRepository : ISuKienRepository
    {
        private readonly ApplicationDbContext _context;

        public SuKienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SuKien>> GetAllAsync()
        {
            return await _context.SuKien.Include(s => s.Khoa).ToListAsync();
        }

        public async Task<SuKien?> GetByIdAsync(int id)
        {
            return await _context.SuKien.Include(s => s.Khoa).FirstOrDefaultAsync(s => s.MaSuKien == id);
        }

        public async Task AddAsync(SuKien suKien)
        {
            _context.SuKien.Add(suKien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuKien suKien)
        {
            _context.SuKien.Update(suKien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var suKien = await _context.SuKien.FindAsync(id);
            if (suKien != null)
            {
                _context.SuKien.Remove(suKien);
                await _context.SaveChangesAsync();
            }
        }
    }
}