using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IWithPersonRepository<TEntity> 
        : IDeeplyDetailedRepository<TEntity> 
        where TEntity : BaseEntity
    {
        Task<TEntity?> GetByPersonId(int id);
    }
}
