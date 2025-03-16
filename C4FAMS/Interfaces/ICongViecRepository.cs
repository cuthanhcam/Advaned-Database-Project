using C4FAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Interfaces
{
    public interface ICongViecRepository
    {
        Task<CongViec> GetByIdAsync(int id);
        Task<IEnumerable<CongViec>> GetByMssvAsync(string mssv);
        Task AddAsync(CongViec congViec);
        Task UpdateAsync(CongViec congViec);
        Task DeleteAsync(int id);
    }
}