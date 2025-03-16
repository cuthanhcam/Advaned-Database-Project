using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IChuyenNganhRepository
    {
        Task<IEnumerable<ChuyenNganh>> GetAllAsync();
        Task<IEnumerable<ChuyenNganh>> GetByKhoaAsync(int maKhoa);
        Task<ChuyenNganh?> GetByIdAsync(int maChuyenNganh);
        Task AddAsync(ChuyenNganh chuyenNganh);
        Task UpdateAsync(ChuyenNganh chuyenNganh);
        Task DeleteAsync(int maChuyenNganh);
    }
}