using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ISuKienRepository
    {
        Task<IEnumerable<SuKien>> GetAllAsync();
        Task<IEnumerable<SuKien>> GetByKhoaAsync(int maKhoa);
        Task<SuKien> GetByIdAsync(int id);
        Task AddAsync(SuKien suKien);
        Task UpdateAsync(SuKien suKien);
        Task DeleteAsync(int id);
    }
}