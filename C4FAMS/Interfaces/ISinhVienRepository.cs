using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ISinhVienRepository
    {
        Task<IEnumerable<SinhVien>> GetAllAsync();
        Task<IEnumerable<SinhVien>> GetByKhoaAsync(int maKhoa);
        Task<SinhVien?> GetByIdAsync(string mssv);
        Task AddAsync(SinhVien sinhVien);
        Task UpdateAsync(SinhVien sinhVien);
        Task DeleteAsync(string mssv);
    }
}