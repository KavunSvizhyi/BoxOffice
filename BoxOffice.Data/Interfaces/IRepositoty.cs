using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IRepositoty<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteByIdAsync(int id);
        void Update(TEntity entity);
    }
}
