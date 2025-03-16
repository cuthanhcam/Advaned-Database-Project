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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<SuKien>> GetAllAsync()
        {
            return await _context.SuKiens
                .Include(s => s.Khoa)
                .Include(s => s.SuKienHinhAnhs)
                .ToListAsync();
        }

        public async Task<IEnumerable<SuKien>> GetByKhoaAsync(int maKhoa)
        {
            return await _context.SuKiens
                .Include(s => s.Khoa)
                .Include(s => s.SuKienHinhAnhs)
                .Include(s => s.SuKienSinhViens) // Bao gồm danh sách sinh viên tham gia (nếu cần)
                .Where(s => s.MaKhoa == maKhoa)
                .ToListAsync();
        }

        public async Task<SuKien?> GetByIdAsync(int maSuKien)
        {
            return await _context.SuKiens
                .Include(s => s.SuKienHinhAnhs)
                .Include(s => s.Khoa)
                .FirstOrDefaultAsync(s => s.MaSuKien == maSuKien);
        }

        public async Task AddAsync(SuKien suKien)
        {
            if (suKien == null) throw new ArgumentNullException(nameof(suKien));
            _context.SuKiens.Add(suKien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuKien suKien)
        {
            if (suKien == null) throw new ArgumentNullException(nameof(suKien));
            _context.SuKiens.Update(suKien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maSuKien)
        {
            var suKien = await GetByIdAsync(maSuKien);
            if (suKien != null)
            {
                _context.SuKiens.Remove(suKien);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SuKien>> GetBySinhVienAsync(string mssv)
        {
            return await _context.SuKiens
                .Include(s => s.Khoa)
                .Include(s => s.SuKienHinhAnhs)
                .Where(s => s.SuKienSinhViens.Any(ss => ss.MSSV == mssv))
                .ToListAsync();
        }
    }
}