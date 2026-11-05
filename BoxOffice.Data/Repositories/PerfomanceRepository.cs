using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly DbSet<Performance> _performances;

        public PerformanceRepository(DbSet<Performance> performances)
        {
            _performances = performances;
        }

        public async Task AddAsync(Performance entity)
        {
            await _performances.AddAsync(entity);
        }

        public void Delete(Performance entity)
        {
            _performances.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var performance = await _performances.FindAsync(id);

            if (performance is not null)
            {
                _performances.Remove(performance);
            }
        }

        public async Task<IEnumerable<Performance>> GetAllAsync()
        {
            return await _performances.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Performance>> GetAllWithDetailsAsync()
        {
            return await _performances
                .Include(x => x.Author)
                .   ThenInclude(x => x!.Person)
                .Include(x => x.Tickets)
                    .ThenInclude(x => x.TicketBooking)
                        .ThenInclude(x => x!.Customer)
                            .ThenInclude(x => x.Person)
                .Include(x => x.Tickets)
                    .ThenInclude(x => x.TicketBooking)
                        .ThenInclude(x => x!.Customer)
                            .ThenInclude(x => x.TicketBookings)
                .ToListAsync();
        }

        public async Task<Performance?> GetByIdAsync(int id)
        {
            return await _performances.FindAsync(id);
        }

        public async Task<Performance?> GetByIdWithDetailsAsync(int id)
        {
            return await _performances
                .Include(x => x.Author)
                    .ThenInclude(x => x!.Person)
                .Include(x => x.Tickets)
                    .ThenInclude(x => x.TicketBooking)
                        .ThenInclude(x => x!.Customer)
                            .ThenInclude(x => x.Person)
                .Include(x => x.Tickets)
                    .ThenInclude(x => x.TicketBooking)
                        .ThenInclude(x => x!.Customer)
                            .ThenInclude(x => x.TicketBookings)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Performance entity)
        {
            _performances.Update(entity);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _performances.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByGenreIdAsync(int genreId)
        {
            return await _performances.AnyAsync(x => x.GenreId == genreId);
        }
    }
}
