using C4FAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C4FAMS.Interfaces
{
    public interface IThanhTuuRepository
    {
        Task<ThanhTuu> GetByIdAsync(int id);
        Task<IEnumerable<ThanhTuu>> GetByMssvAsync(string mssv);
        Task AddAsync(ThanhTuu thanhTuu);
        Task UpdateAsync(ThanhTuu thanhTuu);
        Task DeleteAsync(int id);
    }
}