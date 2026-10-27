using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IDeeplyDetailedRepository<TEntity> : IDetailedRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdWithDetailsAsync(int id);
    }
}
