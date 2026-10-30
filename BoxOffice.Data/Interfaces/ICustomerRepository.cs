using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface ICustomerRepository : IWithPersonRepository<Customer>
    {
    }
}
