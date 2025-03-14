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

        public async Task<IEnumerable<ChuyenNganh>> GetAllByKhoaAsync(int maKhoa)
        {
            return await _context.ChuyenNganh
                .Where(c => c.MaKhoa == maKhoa)
                .ToListAsync();
        }
    }
}