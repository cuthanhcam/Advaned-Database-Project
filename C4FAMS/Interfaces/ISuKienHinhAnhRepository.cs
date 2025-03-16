using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ISuKienHinhAnhRepository
    {
        Task<IEnumerable<SuKienHinhAnh>> GetBySuKienAsync(int maSuKien);
        Task<SuKienHinhAnh> GetByIdAsync(int maHinhAnh);
        Task AddAsync(SuKienHinhAnh suKienHinhAnh);
        Task DeleteAsync(int maHinhAnh);
    }
}