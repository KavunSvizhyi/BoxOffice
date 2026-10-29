using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface ICustomerService : IModelService<CustomerModel>
    {
        Task<IEnumerable<CustomerModel>> GetCustomersByPerformanceIdAsync(int performanceId);
    }
}
