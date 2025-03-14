using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace C4FAMS.Repositories
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly ApplicationDbContext _context;

        public ThongBaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThongBao>> GetAllAsync()
        {
            return await _context.ThongBao.Include(t => t.CuuSinhVien).ThenInclude(c => c.SinhVien).ToListAsync();
        }

        public async Task<ThongBao?> GetByIdAsync(int id)
        {
            return await _context.ThongBao.Include(t => t.CuuSinhVien).FirstOrDefaultAsync(t => t.MaThongBao == id);
        }

        public async Task AddAsync(ThongBao thongBao)
        {
            _context.ThongBao.Add(thongBao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThongBao thongBao)
        {
            _context.ThongBao.Update(thongBao);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var thongBao = await _context.ThongBao.FindAsync(id);
            if (thongBao != null)
            {
                _context.ThongBao.Remove(thongBao);
                await _context.SaveChangesAsync();
            }
        }
    }
}