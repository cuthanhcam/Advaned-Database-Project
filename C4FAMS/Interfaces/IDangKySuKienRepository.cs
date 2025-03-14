using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IDangKySuKienRepository
    {
        Task<IEnumerable<DangKySuKien>> GetAllAsync();
        Task<DangKySuKien?> GetByIdAsync(int id);
        Task AddAsync(DangKySuKien dangKySuKien);
        Task DeleteAsync(int id);
    }
}