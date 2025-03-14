using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IThongBaoRepository
    {
        Task<IEnumerable<ThongBao>> GetAllAsync();
        Task<ThongBao?> GetByIdAsync(int id);
        Task AddAsync(ThongBao thongBao);
        Task UpdateAsync(ThongBao thongBao);
        Task DeleteAsync(int id);
    }
}