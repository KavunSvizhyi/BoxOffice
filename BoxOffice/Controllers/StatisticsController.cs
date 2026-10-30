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
            if (count <= 0)
            {
                return BadRequest($"{nameof(count)} can only be a positive number!");
            }

            return Ok(await _statisticService.GetMostSuccessfulAuthorsAsync(count));
        }

        [HttpGet("most-valuable-customers")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetCustomers(int count)
        {
            if (count <= 0)
            {
                return BadRequest($"{nameof(count)} can only be a positive number!");
            }

            return Ok(await _statisticService.GetMostValuableCustomersAsync(count));
        }

        [HttpGet("income-of-genre/{genreId}")]
        public async Task<ActionResult<decimal>> GetIncome(int genreId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest($"{nameof(startDate)} cannot be after {endDate}!");
            }

            return Ok(await _statisticService.GetIncomeOfGenreInPeriodAsync(genreId, startDate, endDate));
        }
    }
}
