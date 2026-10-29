using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> Get(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet("by-performance/{id}")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetByPerformanceId(int id)
        {
            return Ok(await _customerService.GetCustomersByPerformanceIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Post(CustomerModel model)
        {
            var output = await _customerService.AddAsync(model);

            return CreatedAtAction(nameof(this.Get), new { id = output.Id }, output);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustomerModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            await _customerService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);

            return NoContent();
        }
    }
}
