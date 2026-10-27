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
            return await _people.ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _people.FindAsync(id);
        }

        public void Update(Person entity)
        {
            _people.Update(entity);
        }
    }
}
