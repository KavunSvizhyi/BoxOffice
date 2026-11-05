using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IWithPersonRepository<TEntity> 
        : IDetailedRepository<TEntity> 
        where TEntity : BaseEntity
    {
        Task<bool> ExistsByPersonId(int personId);
    }
}
