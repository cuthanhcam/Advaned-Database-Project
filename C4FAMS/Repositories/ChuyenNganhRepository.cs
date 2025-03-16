using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class ChuyenNganhRepository : IChuyenNganhRepository
    {
        private readonly ApplicationDbContext _context;

        public ChuyenNganhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChuyenNganh>> GetAllAsync()
        {
            return await _context.ChuyenNganh.Include(c => c.Khoa).ToListAsync();
        }

        public async Task<IEnumerable<ChuyenNganh>> GetByKhoaAsync(int maKhoa)
        {
            return await _context.ChuyenNganh
                .Where(c => c.MaKhoa == maKhoa)
                .ToListAsync();
        }
        
        public async Task<ChuyenNganh?> GetByIdAsync(int maChuyenNganh)
        {
            return await _context.ChuyenNganh
                .FirstOrDefaultAsync(c => c.MaChuyenNganh == maChuyenNganh);
        }

        public async Task AddAsync(ChuyenNganh chuyenNganh)
        {
            _context.ChuyenNganh.Add(chuyenNganh);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChuyenNganh chuyenNganh)
        {
            _context.ChuyenNganh.Update(chuyenNganh);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maChuyenNganh)
        {
            var chuyenNganh = await _context.ChuyenNganh.FindAsync(maChuyenNganh);
            if (chuyenNganh != null)
            {
                _context.ChuyenNganh.Remove(chuyenNganh);
                await _context.SaveChangesAsync();
            }
        }
    }
}