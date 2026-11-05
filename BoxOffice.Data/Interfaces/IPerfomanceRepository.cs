using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IPerformanceRepository : IDetailedRepository<Performance>
    {
        Task<bool> ExistsByGenreIdAsync(int genreId);
    }
}
