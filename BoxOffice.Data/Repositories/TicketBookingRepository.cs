using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class TicketBookingRepository : ITicketBookingRepository
    {
        private readonly DbSet<TicketBooking> _ticketBookings;

        public TicketBookingRepository(DbSet<TicketBooking> ticketBookings)
        {
            _ticketBookings = ticketBookings;
        }

        public async Task AddAsync(TicketBooking entity)
        {
            await _ticketBookings.AddAsync(entity);
        }

        public void Delete(TicketBooking entity)
        {
            _ticketBookings.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var ticketBooking = await _ticketBookings.FindAsync(id);

            if (ticketBooking is not null)
            {
                _ticketBookings.Remove(ticketBooking);
            }
        }

        public async Task<IEnumerable<TicketBooking>> GetAllAsync()
        {
            return await _ticketBookings.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TicketBooking>> GetAllWithDetailsAsync()
        {
            return await _ticketBookings
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Ticket)
                .ToListAsync();
        }

        public async Task<TicketBooking?> GetByIdAsync(int id)
        {
            return await _ticketBookings.FindAsync(id);
        }

        public void Update(TicketBooking entity)
        {
            _ticketBookings.Update(entity);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _ticketBookings.AnyAsync(x => x.Id == id);
        }
    }
}
