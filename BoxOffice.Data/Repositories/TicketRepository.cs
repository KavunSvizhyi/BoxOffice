using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DbSet<Ticket> _tickets;

        public TicketRepository(DbSet<Ticket> tickets)
        {
            _tickets = tickets;
        }

        public async Task AddAsync(Ticket entity)
        {
            await _tickets.AddAsync(entity);
        }

        public void Delete(Ticket entity)
        {
            _tickets.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var ticket = await _tickets.FindAsync(id);

            if (ticket is not null)
            {
                _tickets.Remove(ticket);
            }
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _tickets.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllWithDetailsAsync()
        {
            return await _tickets
                .AsNoTracking()
                .Include(x => x.TicketBooking)
                .Include(x => x.Performance)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _tickets.FindAsync(id);
        }

        public async Task<Ticket?> GetByIdWithDetailsAsync(int id)
        {
            return await _tickets
                .Include(x => x.TicketBooking)
                .Include(x => x.Performance)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Ticket entity)
        {
            _tickets.Update(entity);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _tickets.AnyAsync(x => x.Id == id);
        }
    }
}
