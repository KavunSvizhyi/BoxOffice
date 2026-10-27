using BoxOffice.Data.Interfaces;
using BoxOffice.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TheatreDbContext _context;

        public UnitOfWork(TheatreDbContext context)
        {
            _context = context;
        }

        public IPersonRepository PersonRepository => field ?? new PersonRepository(_context.People);

        public IAuthorRepository AuthorRepository => field ?? new AuthorRepository(_context.Authors);

        public ICustomerRepository CustomerRepository => field ?? new CustomerRepository(_context.Customers);

        public IPerfomanceRepository PerfomanceRepository => field ?? new PerfomanceRepository(_context.Perfomances);

        public IGenreRepository GenreRepository => field ?? new GenreRepository(_context.Genres);

        public ITicketRepository TicketRepository => field ?? new TicketRepository(_context.Tickets);

        public ITicketBookingRepository TicketBookingRepository => field ?? new TicketBookingRepository(_context.TicketBookings);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
