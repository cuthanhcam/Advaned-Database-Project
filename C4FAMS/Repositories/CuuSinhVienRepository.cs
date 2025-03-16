using C4FAMS.Data;
using C4FAMS.Interfaces;
using C4FAMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Repositories
{
    public class CuuSinhVienRepository : ICuuSinhVienRepository
    {
        private readonly ApplicationDbContext _context;

        public CuuSinhVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CuuSinhVien> GetByMssvAsync(string mssv)
        {
            return await _context.CuuSinhVien
                .Include(c => c.SinhVien)
                .ThenInclude(sv => sv.ChuyenNganh)
                .Include(c => c.CongViec)
                .Include(c => c.ThanhTuu)
                .FirstOrDefaultAsync(c => c.MSSV == mssv);
        }

        public async Task<IEnumerable<CuuSinhVien>> GetAllAsync()
        {
            return await _context.CuuSinhVien
                .Include(c => c.SinhVien)
                .Include(c => c.CongViec)
                .Include(c => c.ThanhTuu)
                .ToListAsync();
        }

        public async Task<IEnumerable<CuuSinhVien>> GetByKhoaAsync(int maKhoa)
        {
            return await _context.CuuSinhVien
                .Include(c => c.SinhVien)
                .ThenInclude(s => s.ChuyenNganh)
                .Where(c => c.SinhVien.ChuyenNganh.MaKhoa == maKhoa)
                .ToListAsync();
        }

        public async Task AddAsync(CuuSinhVien cuuSinhVien)
        {
            _context.CuuSinhVien.Add(cuuSinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CuuSinhVien cuuSinhVien)
        {
            _context.CuuSinhVien.Update(cuuSinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string mssv)
        {
            var cuuSinhVien = await _context.CuuSinhVien.FindAsync(mssv);
            if (cuuSinhVien != null)
            {
                _context.CuuSinhVien.Remove(cuuSinhVien);
                await _context.SaveChangesAsync();
            }
        }
    }
}