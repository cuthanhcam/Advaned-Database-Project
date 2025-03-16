using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Repositories
{
    public class ThanhTuuRepository : IThanhTuuRepository
    {
        private readonly ApplicationDbContext _context;

        public ThanhTuuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ThanhTuu> GetByIdAsync(int id)
        {
            return await _context.ThanhTuu.FindAsync(id);
        }

        public async Task<IEnumerable<ThanhTuu>> GetByMssvAsync(string mssv)
        {
            return await _context.ThanhTuu
                .Where(t => t.MSSV == mssv)
                .ToListAsync();
        }

        public async Task AddAsync(ThanhTuu thanhTuu)
        {
            _context.ThanhTuu.Add(thanhTuu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThanhTuu thanhTuu)
        {
            _context.ThanhTuu.Update(thanhTuu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var thanhTuu = await _context.ThanhTuu.FindAsync(id);
            if (thanhTuu != null)
            {
                _context.ThanhTuu.Remove(thanhTuu);
                await _context.SaveChangesAsync();
            }
        }
    }
}