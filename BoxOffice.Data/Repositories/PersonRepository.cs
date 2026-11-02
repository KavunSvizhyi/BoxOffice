using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DbSet<Person> _people;

        public PersonRepository(DbSet<Person> people)
        {
            _people = people; 
        }

        public async Task AddAsync(Person entity)
        {
            await _people.AddAsync(entity);
        }

        public void Delete(Person entity)
        {
            _people.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var person = await _people.FindAsync(id);

            if (person is not null)
            {
                _people.Remove(person);
            }
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _people.AsNoTracking().ToListAsync();
        }

        public async Task<Person?> GetByEmailAsync(string email)
        {
            return await _people.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _people.FindAsync(id);
        }

        public void Update(Person entity)
        {
            _people.Update(entity);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _people.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _people.AnyAsync(x => x.Email == email);
        }
    }
}
