using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("most-successful-authors")]
        public async Task<ActionResult<IEnumerable<AuthorActivityModel>>> GetAuthors(int count)
        {
            return Ok(await _statisticService.GetMostSuccessfulAuthorsAsync(count));
        }

        [HttpGet("most-valuable-customers")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetCustomers(int count)
        {
            return Ok(await _statisticService.GetMostValuableCustomersAsync(count));
        }

        [HttpGet("income-of-genre")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetIncome(int count, DateTime startDate, DateTime endDate)
        {
            return Ok(await _statisticService.GetIncomeOfGenreInPeriodAsync(count, startDate, endDate));
        }
    }
}
