using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class PerfomanceRepository : IPerfomanceRepository
    {
        private readonly DbSet<Perfomance> _perfomances;

        public PerfomanceRepository(DbSet<Perfomance> perfomances)
        {
            _perfomances = perfomances;
        }

        public async Task AddAsync(Perfomance entity)
        {
            await _perfomances.AddAsync(entity);
        }

        public void Delete(Perfomance entity)
        {
            _perfomances.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var perfomance = await _perfomances.FindAsync(id);

            if (perfomance is not null)
            {
                _perfomances.Remove(perfomance);
            }
        }

        public async Task<IEnumerable<Perfomance>> GetAllAsync()
        {
            return await _perfomances.ToListAsync();
        }

        public async Task<IEnumerable<Perfomance>> GetAllWithDetailsAsync()
        {
            return await _perfomances
                .Include(x => x.Author)
                .ThenInclude(x => x!.Person)
                .Include(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .ThenInclude(x => x!.Customer)
                .ToListAsync();
        }

        public async Task<Perfomance?> GetByIdAsync(int id)
        {
            return await _perfomances.FindAsync(id);
        }

        public async Task<Perfomance?> GetByIdWithDetailsAsync(int id)
        {
            return await _perfomances
                .Include(x => x.Author)
                .ThenInclude(x => x!.Person)
                .Include(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .ThenInclude(x => x!.Customer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Perfomance entity)
        {
            _perfomances.Update(entity);
        }
    }
}
