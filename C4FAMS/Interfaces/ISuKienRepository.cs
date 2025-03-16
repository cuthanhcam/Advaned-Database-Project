using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ISuKienRepository
    {
        Task<IEnumerable<SuKien>> GetAllAsync();
        Task<IEnumerable<SuKien>> GetBySinhVienAsync(string mssv);
        Task<IEnumerable<SuKien>> GetByKhoaAsync(int maKhoa);
        Task<SuKien?> GetByIdAsync(int maSuKien);
        Task AddAsync(SuKien suKien);
        Task UpdateAsync(SuKien suKien);
        Task DeleteAsync(int maSuKien);
    }
}