using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformancesController : ControllerBase
    {
        private readonly IPerformanceService _performanceService;

        public PerformancesController(IPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerformanceModel>>> Get()
        {
            return Ok(await _performanceService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PerformanceModel>> Get(int id)
        {
            var performance = await _performanceService.GetByIdAsync(id);

            if (performance is null)
            {
                return NotFound();
            }

            return Ok(performance);
        }

        [HttpGet("by-filter")]
        public async Task<ActionResult<IEnumerable<PerformanceModel>>> Get(SearchFilterModel filter)
        {
            return Ok(await _performanceService.GetByFilterAsync(filter));
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PerformanceModel>>> GetAvailable()
        {
            return Ok(await _performanceService.GetAllAvailablePerformancesAsync());
        }

        [HttpGet("by-customer/{id}")]
        public async Task<ActionResult<IEnumerable<PerformanceModel>>> GetByCustomer(int id)
        {
            return Ok(await _performanceService.GetPerformancesByCustomerIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<PerformanceModel>>> Post(PerformanceModel model)
        {
            var output = await _performanceService.AddAsync(model);

            return CreatedAtAction(nameof(this.Get), new { id = output.Id }, output);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PerformanceModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            await _performanceService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _performanceService.DeleteAsync(id);

            return NoContent();
        }
    }
}
