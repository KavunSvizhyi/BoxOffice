using AutoMapper;
using Azure.Core;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Services;
using BoxOffice.Data.Interfaces;
using BoxOffice.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public TicketsController(ITicketService ticketService, IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketModel>>> Get()
        {
            return Ok(await _ticketService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketModel>> Get(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket is null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpGet("available/{performanceId}")]
        public async Task<ActionResult<IEnumerable<TicketModel>>> GetAvailableByPerformanceId(int performanceId)
        {
            return Ok(await _ticketService.GetAvailableTicketsByPerformanceIdAsync(performanceId));
        }

        [HttpGet("bookings/by-customer/{id}")]
        public async Task<ActionResult<IEnumerable<TicketBookingModel>>> GetBookingsByCustomer(int id)
        {
            return Ok(await _ticketService.GetBookingsByCustomerIdAsync(id));
        }

        [HttpGet("bookings/by-ticket/{id}")]
        public async Task<ActionResult<TicketBookingModel?>> GetBookingByTicketId(int id)
        {
            var booking = await _ticketService.GetBookingDetailsByTicketIdAsync(id);

            if (booking is null)
            {
                return NotFound();
            }

            return Ok(await _ticketService.GetBookingDetailsByTicketIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<TicketPostResponse>> Post(AddTicketRequest request)
        {
            var model = _mapper.Map<TicketModel>(request);

            var output = await _ticketService.AddAsync(model);

            var response = _mapper.Map<TicketPostResponse>(output);

            return CreatedAtAction(nameof(this.Get), new { id = response.Id }, response);
        }

        [HttpPost("bookings")]
        public async Task<ActionResult<TicketBookingModel>> Post(AddTicketBookingRequest request)
        {
            var model = _mapper.Map<TicketBookingModel>(request);

            var output = await _ticketService.BookTicketAsync(model.TicketId, model.CustomerId);

            return CreatedAtAction(nameof(this.GetBookingByTicketId), new { id = output.TicketId }, output);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TicketModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            await _ticketService.UpdateAsync(model);

            return NoContent();
        }

        [HttpPatch("selling/{id}")]
        public async Task<IActionResult> Patch(int id)
        {
            await _ticketService.ChangeToSoldAsync(id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);

            return NoContent();
        }

        [HttpDelete("booking/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _ticketService.CancelBookingAsync(id);

            return NoContent();
        }
    }
}
