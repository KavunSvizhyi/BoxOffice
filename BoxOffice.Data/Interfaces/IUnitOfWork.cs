using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Interfaces
{
    public interface IUnitOfWork 
    {
        IPersonRepository PersonRepository { get; }
        IAuthorRepository AuthorRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IPerfomanceRepository PerfomanceRepository { get; }
        IGenreRepository GenreRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITicketBookingRepository TicketBookingRepository { get; }
        Task SaveAsync();
    }
}
