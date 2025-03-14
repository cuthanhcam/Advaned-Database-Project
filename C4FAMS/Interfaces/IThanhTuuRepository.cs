using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IThanhTuuRepository
    {
        Task<IEnumerable<ThanhTuu>> GetAllAsync();
        Task<ThanhTuu?> GetByIdAsync(int id);
        Task AddAsync(ThanhTuu thanhTuu);
        Task UpdateAsync(ThanhTuu thanhTuu);
        Task DeleteAsync(int id);
    }
}