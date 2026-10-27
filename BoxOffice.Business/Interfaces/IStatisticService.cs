using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface IStatisticService
    {
        Task<IEnumerable<PerfomanceModel>> GetMostPopularPerfomancesAsync(int count);

        Task<IEnumerable<AuthorActivityModel>> GetMostSuccessfulAuthorsAsync(int count);

        Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int count);

        Task<decimal> GetIncomeOfGenreInPeriodAsync(int genreId, DateTime startDate, DateTime endTime);
    }
}
