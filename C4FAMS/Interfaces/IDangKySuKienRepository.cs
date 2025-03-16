using C4FAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Interfaces
{
    public interface IDangKySuKienRepository
    {
        Task<IEnumerable<DangKySuKien>> GetAllAsync();
        Task<DangKySuKien> GetByIdAsync(int id);
        Task<IEnumerable<DangKySuKien>> GetBySuKienAsync(int maSuKien);
        Task<IEnumerable<DangKySuKien>> GetByCuuSinhVienAsync(string mssv);
        Task AddAsync(DangKySuKien dangKy);
        Task UpdateAsync(DangKySuKien dangKy);
        Task DeleteAsync(int id);
    }
}