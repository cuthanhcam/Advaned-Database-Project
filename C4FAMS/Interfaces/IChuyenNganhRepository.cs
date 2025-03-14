using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C4FAMS.Models;

namespace C4FAMS.Interfaces
{
    public interface IChuyenNganhRepository
    {
        Task<IEnumerable<ChuyenNganh>> GetAllByKhoaAsync(int maKhoa);
    }
}