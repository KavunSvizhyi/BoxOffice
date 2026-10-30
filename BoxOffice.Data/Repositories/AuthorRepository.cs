using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DbSet<Author> _authors;

        public AuthorRepository(DbSet<Author> authors)
        {
            _authors = authors;
        }

        public async Task AddAsync(Author entity)
        {
            await _authors.AddAsync(entity);
        }

        public void Delete(Author entity)
        {
            _authors.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var author = await _authors.FindAsync(id);

            if (author is not null)
            {
                _authors.Remove(author);
            }
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _authors.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAllWithDetailsAsync()
        {
            return await _authors
                .AsNoTracking()
                .Include(x => x.Person)
                .Include(x => x.Performances)
                .ThenInclude(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _authors.FindAsync(id);
        }

        public async Task<Author?> GetByIdWithDetailsAsync(int id)
        {
            return await _authors
                .Include(x => x.Person)
                .Include(x => x.Performances)
                .ThenInclude(x => x.Tickets)
                .ThenInclude(x => x.TicketBooking)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Author?> GetByPersonId(int personId)
        {
            return await _authors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PersonId == personId);
        }

        public void Update(Author entity)
        {
            _authors.Update(entity);
        }
    }
}
