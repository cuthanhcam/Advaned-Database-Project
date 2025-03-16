using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class KhoaRepository : IKhoaRepository
    {
        private readonly ApplicationDbContext _context;

        public KhoaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Khoa>> GetAllAsync()
        {
            return await _context.Khoa.ToListAsync();
        }

        public async Task<Khoa?> GetByIdAsync(int id)
        {
            return await _context.Khoa
                .Include(k => k.ChuyenNganh)
                .FirstOrDefaultAsync(k => k.MaKhoa == id);
        }

        public async Task AddAsync(Khoa khoa)
        {
            _context.Khoa.Add(khoa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Khoa khoa)
        {
            _context.Khoa.Update(khoa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var khoa = await _context.Khoa
                .Include(k => k.ChuyenNganh)
                .FirstOrDefaultAsync(k => k.MaKhoa == id);
            if (khoa == null) return;
            if (khoa.ChuyenNganh.Any())
            {
                throw new InvalidOperationException("Không thể xóa khoa vì còn chuyên ngành liên quan.");
            }
            _context.Khoa.Remove(khoa);
            await _context.SaveChangesAsync();
        }
    }
}