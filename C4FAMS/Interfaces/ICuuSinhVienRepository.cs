using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ICuuSinhVienRepository
    {
        Task<IEnumerable<CuuSinhVien>> GetAllAsync();
        Task<CuuSinhVien?> GetByIdAsync(string mssv);
        Task AddAsync(CuuSinhVien cuuSinhVien);
        Task UpdateAsync(CuuSinhVien cuuSinhVien);
        Task DeleteAsync(string mssv);
    }
}