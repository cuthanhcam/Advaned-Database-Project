using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ICongViecRepository
    {
        Task<IEnumerable<CongViec>> GetAllAsync();
        Task<CongViec?> GetByIdAsync(int id);
        Task AddAsync(CongViec congViec);
        Task UpdateAsync(CongViec congViec);
        Task DeleteAsync(int id);
    }
}