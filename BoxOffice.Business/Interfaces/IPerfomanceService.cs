using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface IPerfomanceService : IModelService<PerfomanceModel>
    {
        Task<IEnumerable<PerfomanceModel>> GetByFilterAsync(SearchFilterModel filter);

        Task<IEnumerable<PerfomanceModel>> GetAllAvailablePerfomancesAsync();

        Task<IEnumerable<PerfomanceModel>> GetPerfomancesByCustomerIdAsync(int customerId);
    }
}
