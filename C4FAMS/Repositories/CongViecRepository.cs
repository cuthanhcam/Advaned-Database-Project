using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Repositories
{
    public class CongViecRepository : ICongViecRepository
    {
        private readonly ApplicationDbContext _context;

        public CongViecRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CongViec> GetByIdAsync(int id)
        {
            return await _context.CongViec.FindAsync(id);
        }

        public async Task<IEnumerable<CongViec>> GetByMssvAsync(string mssv)
        {
            return await _context.CongViec
                .Where(c => c.MSSV == mssv)
                .ToListAsync();
        }

        public async Task AddAsync(CongViec congViec)
        {
            _context.CongViec.Add(congViec);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CongViec congViec)
        {
            _context.CongViec.Update(congViec);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var congViec = await _context.CongViec.FindAsync(id);
            if (congViec != null)
            {
                _context.CongViec.Remove(congViec);
                await _context.SaveChangesAsync();
            }
        }
    }
}