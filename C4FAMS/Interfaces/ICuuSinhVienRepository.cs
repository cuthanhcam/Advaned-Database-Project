using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ICuuSinhVienRepository
    {
        Task<CuuSinhVien> GetByMssvAsync(string mssv);
        Task<IEnumerable<CuuSinhVien>> GetAllAsync();
        Task<IEnumerable<CuuSinhVien>> GetByKhoaAsync(int maKhoa);
        Task AddAsync(CuuSinhVien cuuSinhVien);
        Task UpdateAsync(CuuSinhVien cuuSinhVien);
        Task DeleteAsync(string mssv);
    }
}