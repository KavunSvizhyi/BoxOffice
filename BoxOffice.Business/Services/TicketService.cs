using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Services
{
    public class TicketService : ITicketService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketModel> AddAsync(TicketModel model)
        {
            await ValidateAsync(model);

            var ticket = _mapper.Map<Ticket>(model);
            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<TicketModel>(ticket);
        }

        public async Task<TicketBookingModel> BookTicketAsync(int id, int customerId)
        {
            await ValidateBookingAsync(id, customerId);

            var booking = _mapper.Map<TicketBooking>(new TicketBookingModel()
            {
                TicketId = id,
                CustomerId = customerId,
                OperationDate = DateTime.Now
            });

            await _unitOfWork.TicketBookingRepository.AddAsync(booking);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<TicketBookingModel>(booking);
        }

        private async Task ValidateBookingAsync(int id, int customerId)
        {
            var ticket = _unitOfWork.TicketRepository.GetByIdAsync(id);
            var customer = _unitOfWork.CustomerRepository.GetByIdAsync(customerId);

            if (ticket is null)
            {
                throw new TheatreException($"Ticket with this ID doesn't exist!");
            }

            if (customer is null)
            {
                throw new TheatreException($"Customer with this ID doesn't exist!");
            }
        }

        public async Task CancelBookingAsync(int ticketBookingId)
        {
            var booking = await _unitOfWork.TicketBookingRepository.GetByIdAsync(ticketBookingId);
            if (booking is null)
            {
                throw new TheatreException($"There is no such ID as {ticketBookingId}!");
            }
            if (booking.IsSold)
            {
                throw new TheatreException($"Cannot cancel an already sold ticket!");
            }

            await _unitOfWork.TicketBookingRepository.DeleteByIdAsync(ticketBookingId);
            await _unitOfWork.SaveAsync();
        }

        public async Task ChangeToSoldAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(id);
            if (ticket is null)
            {
                throw new TheatreException($"There is no such ID as {id}!");
            }

            ValidateSellingTheTicket(id, ticket);

            ticket.TicketBooking!.IsSold = true;

            _unitOfWork.TicketBookingRepository.Update(ticket.TicketBooking);
            await _unitOfWork.SaveAsync();
        }

        private static void ValidateSellingTheTicket(int id, Ticket ticket)
        {
            if (ticket.Perfomance.EventDate < DateTime.Now)
            {
                throw new TheatreException($"Cannot sell ticket to a perfomance, that already happend!");
            }
            if (ticket.TicketBooking is null)
            {
                throw new TheatreException($"The ticket with ID = {id} is not booked yet!");
            }
            if (ticket.TicketBooking.IsSold)
            {
                throw new TheatreException($"Cannot cancel an already sold ticket (ID = {id})!");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(id);
            if (ticket is not null &&
                ticket.TicketBooking is not null)
            {
                throw new TheatreException($"Cannot delete a booked ticket (ID = {id})!");
            }

            await _unitOfWork.TicketRepository.DeleteByIdAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<TicketModel>> GetAllAsync()
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<TicketModel>>(tickets);
        }

        public async Task<IEnumerable<TicketModel>> GetAvailableTicketsByPerfomanceIdAsync(int perfomanceId)
        {
            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdWithDetailsAsync(perfomanceId);
            if (perfomance is null)
            {
                throw new TheatreException($"There is no such ID as {perfomanceId}!");
            }

            if (perfomance.EventDate < DateTime.Now)
            {
                return [];
            }

            var tickets = perfomance.Tickets.Where(t => t.TicketBooking is null);

            return _mapper.Map<IEnumerable<TicketModel>>(tickets);
        }

        public async Task<TicketBookingModel?> GetBookingDetailsByTicketIdAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(id);
            if (ticket is null)
            {
                throw new TheatreException($"There is no such ID as {id}!");
            }

            return _mapper.Map<TicketBookingModel>(ticket.TicketBooking);
        }

        public async Task<IEnumerable<TicketBookingModel>> GetBookingsByCustomerIdAsync(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(customerId);
            if (customer is null)
            {
                throw new TheatreException($"There is no such ID as {customerId}!");
            }

            return _mapper.Map<IEnumerable<TicketBookingModel>>(customer.TicketBookings);
        }

        public async Task<TicketModel?> GetByIdAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<TicketModel>(ticket);
        }

        public async Task UpdateAsync(TicketModel model)
        {
            await ValidateAsync(model);

            var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(model.Id);
            if (ticket is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            ticket.Price = model.Price;
            ticket.PerfomanceId = model.PerfomanceId;
            ticket.SeatName = model.SeatName;

            _unitOfWork.TicketRepository.Update(ticket);
            await _unitOfWork.SaveAsync();
        }

        private async Task ValidateAsync(TicketModel model)
        {
            if (string.IsNullOrEmpty(model.SeatName) ||
                string.IsNullOrWhiteSpace(model.SeatName))
            {
                throw new TheatreException($"{nameof(model.SeatName)} cannot be blank!");
            }

            if (model.Price < 0)
            {
                throw new TheatreException($"{nameof(model.Price)} cannot be a negative number!");
            }

            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdAsync(model.PerfomanceId);
            if (perfomance is null)
            {
                throw new TheatreException($"Perfomance with this {nameof(model.PerfomanceId)} doesn't exist!");
            }

            if (model.TicketBookingId is not null)
            {
                var booking = await _unitOfWork.TicketBookingRepository.GetByIdAsync((int)model.TicketBookingId);
                if (booking is null)
                {
                    throw new TheatreException($"TicketBooking with this {nameof(model.TicketBookingId)} doesn't exist!");
                }
            }
        }
    }
}
