using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IKhoaRepository
    {
        Task<IEnumerable<Khoa>> GetAllAsync();
        Task<Khoa?> GetByIdAsync(int id);
        Task AddAsync(Khoa khoa);
        Task UpdateAsync(Khoa khoa);
        Task DeleteAsync(int id);
    }
}