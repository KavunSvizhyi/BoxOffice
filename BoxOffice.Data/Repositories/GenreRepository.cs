using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly DbSet<Genre> _genres;

        public GenreRepository(DbSet<Genre> genres)
        {
            _genres = genres;
        }

        public async Task AddAsync(Genre entity)
        {
            await _genres.AddAsync(entity);
        }

        public void Delete(Genre entity)
        {
            _genres.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var genre = await _genres.FindAsync(id);

            if (genre is not null)
            {
                _genres.Remove(genre);
            }
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _genres.ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetAllWithDetailsAsync()
        {
            return await _genres
                .Include(x => x.Performances)
                .ThenInclude(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _genres.FindAsync(id);
        }

        public async Task<Genre?> GetByIdWithDetailsAsync(int id)
        {
            return await _genres
                .Include(x => x.Performances)
                .ThenInclude(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Genre entity)
        {
            _genres.Update(entity);
        }
    }
}
