using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.DTO.AddRequest;
using BoxOffice.DTO.PostResponse;
using BoxOffice.DTO.UpdateRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
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
        public async Task<ActionResult<PostPersonResponse>> Post(AddPersonRequest request)
        {
            var model = _mapper.Map<CustomerModel>(request);

            var output = await _customerService.AddAsync(model);

            var response = _mapper.Map<PostPersonResponse>(output);

            return CreatedAtAction(nameof(this.Get), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdatePersonRequest request)
        {
            if (request.Id != id)
            {
                return BadRequest();
            }

            var model = _mapper.Map<CustomerModel>(request);

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
