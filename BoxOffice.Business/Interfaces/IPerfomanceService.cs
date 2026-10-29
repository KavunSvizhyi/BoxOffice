using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface IPerformanceService : IModelService<PerformanceModel>
    {
        Task<IEnumerable<PerformanceModel>> GetByFilterAsync(SearchFilterModel filter);

        Task<IEnumerable<PerformanceModel>> GetAllAvailablePerformancesAsync();

        Task<IEnumerable<PerformanceModel>> GetPerformancesByCustomerIdAsync(int customerId);
    }
}
