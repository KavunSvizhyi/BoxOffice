using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IPersonRepository : IRepositoty<Person>
    {
        public Task<Person?> GetByEmailAsync(string email);
    }
}
