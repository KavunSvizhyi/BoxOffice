using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbSet<Customer> _customers;

        public CustomerRepository(DbSet<Customer> customers)
        {
            _customers = customers;
        }

        public async Task AddAsync(Customer entity)
        {
            await _customers.AddAsync(entity);
        }

        public void Delete(Customer entity)
        {
            _customers.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var customer = await _customers.FindAsync(id);

            if (customer is not null)
            {
                _customers.Remove(customer);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customers.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await _customers
                .AsNoTracking()
                .Include(x => x.Person)
                .Include(x => x.TicketBookings)
                    .ThenInclude(x => x.Ticket)
                        .ThenInclude(x => x.Performance)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _customers.FindAsync(id);
        }

        public async Task<Customer?> GetByIdWithDetailsAsync(int id)
        {
            return await _customers
                .Include(x => x.Person)
                .Include(x => x.TicketBookings)
                    .ThenInclude(x => x.Ticket)
                        .ThenInclude(x => x.Performance)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByPersonId(int personId)
        {
            return await _customers.AnyAsync(x => x.PersonId == personId);
        }

        public void Update(Customer entity)
        {
            _customers.Update(entity);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _customers.AnyAsync(x => x.Id == id);
        }
    }
}
