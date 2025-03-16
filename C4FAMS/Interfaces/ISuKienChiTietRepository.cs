using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface ISuKienChiTietRepository
    {
        Task<SuKienChiTiet> GetByIdAsync(int maSuKien);
        Task AddAsync(SuKienChiTiet suKienChiTiet);
        Task UpdateAsync(SuKienChiTiet suKienChiTiet);
        Task DeleteAsync(int maSuKien);
    }
}