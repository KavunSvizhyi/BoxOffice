using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface IModelService<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel?> GetByIdAsync(int id);

        Task<TModel> AddAsync(TModel model);

        Task UpdateAsync(TModel model);

        Task DeleteAsync(int id);
    }
}
