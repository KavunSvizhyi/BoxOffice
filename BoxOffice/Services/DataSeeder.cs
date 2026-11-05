using BoxOffice.Data.Data;
using BoxOffice.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoxOffice.WebApi.Services
{
    public class DataSeeder
    {
        public void Seed(TheatreDbContext context)
        {
            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }
            else
            {
                _ = context.Database.EnsureCreated();
            }

            if (context.People.Any())
            {
                return;
            }

            #region DummyData

            #region People
            var people = new List<Person>()
            {
                new Person()
                {
                    FirstName = "William",
                    LastName = "Shakespeare",
                    BirthDate = DateTime.Parse("1985-04-23T00:00:00"),
                    Email = "williamshakespeare@gmail.com"
                },
                new Person()
                {
                    FirstName = "Anton",
                    LastName = "Bulba",
                    BirthDate = DateTime.Parse("1990-01-29T00:00:00"),
                    Email = "antonbulba@gmail.com"
                },
                new Person()
                {
                    FirstName = "Oscar",
                    LastName = "Wilde",
                    BirthDate = DateTime.Parse("1982-10-16T00:00:00"),
                    Email = "oscarwilde@gmail.com"
                },
                new Person()
                {
                    FirstName = "Arthur",
                    LastName = "Miller",
                    BirthDate = DateTime.Parse("1978-10-17T00:00:00"),
                    Email = "arthurmiller@gmail.com"
                },
                new Person()
                {
                    FirstName = "Emma",
                    LastName = "Johnson",
                    BirthDate = DateTime.Parse("1995-05-14T00:00:00"),
                    Email = "emmajohnson@gmail.com"
                },
                new Person()
                {
                    FirstName = "James",
                    LastName = "Smith",
                    BirthDate = DateTime.Parse("1988-11-02T00:00:00"),
                    Email = "jamessmith@gmail.com"
                },
                new Person()
                {
                    FirstName = "Olivia",
                    LastName = "Brown",
                    BirthDate = DateTime.Parse("2000-01-30T00:00:00"),
                    Email = "oliviabrown@gmail.com"
                },
                new Person()
                {
                    FirstName = "Daniel",
                    LastName = "Taylor",
                    BirthDate = DateTime.Parse("1992-07-21T00:00:00"),
                    Email = "danieltaylor@gmail.com"
                }
            };
            #endregion

            #region Authors
            var authors = new List<Author>()
            {
                new Author()
                {
                    PersonId = 1
                },
                new Author()
                {
                    PersonId = 2
                },
                new Author()
                {
                    PersonId = 3
                },
                new Author()
                {
                    PersonId = 4
                }
            };
            #endregion

            #region Customers
            var customers = new List<Customer>()
            {
                new Customer()
                {
                    PersonId = 5
                },
                new Customer()
                {
                    PersonId = 6
                },
                new Customer()
                {
                    PersonId = 7
                },
                new Customer()
                {
                    PersonId = 8
                }
            };
            #endregion

            #region Genres
            var genres = new List<Genre>()
            {
                new Genre()
                {
                    GenreName = "Drama"
                },
                new Genre()
                {
                    GenreName = "Comedy"
                },
                new Genre()
                {
                    GenreName = "Musical"
                },
                new Genre()
                {
                    GenreName = "Tragedy"
                }
            };
            #endregion

            #region Performances
            var performances = new List<Performance>()
            {
                new Performance()
                {
                    PerformanceName = "Hamlet",
                    AuthorId = 1,
                    GenreId = 4,
                    EventDate = DateTime.Parse("2026-11-05T19:00:00")
                },
                new Performance()
                {
                    PerformanceName = "The Cherry Orchard",
                    AuthorId = 2,
                    GenreId = 1,
                    EventDate = DateTime.Parse("2026-11-12T18:30:00")
                },
                new Performance()
                {
                    PerformanceName = "The Importance of Being Earnest",
                    AuthorId = 3,
                    GenreId = 2,
                    EventDate = DateTime.Parse("2026-11-20T19:00:00")
                },
                new Performance()
                {
                    PerformanceName = "Death of a Salesman",
                    AuthorId = 4,
                    GenreId = 1,
                    EventDate = DateTime.Parse("2026-12-01T19:30:00")
                },
                new Performance()
                {
                    PerformanceName = "A Midsummer Night's Dream",
                    AuthorId = 1,
                    GenreId = 2,
                    EventDate = DateTime.Parse("2026-12-10T18:00:00")
                },
                new Performance()
                {
                    PerformanceName = "The Crucible",
                    AuthorId = 4,
                    GenreId = 4,
                    EventDate = DateTime.Parse("2026-12-18T19:00:00")
                }
            };
            #endregion

            #region Ticket
            var tickets = new List<Ticket>()
            {
                new Ticket()
                {
                    SeatName = "A1",
                    PerformanceId = 1,
                    Price = 250
                },
                new Ticket()
                {
                    SeatName = "B4",
                    PerformanceId = 1,
                    Price = 350
                },
                new Ticket()
                {
                    SeatName = "A2",
                    PerformanceId = 2,
                    Price = 400
                },
                new Ticket()
                {
                    SeatName = "B5",
                    PerformanceId = 2,
                    Price = 500
                },
                new Ticket()
                {
                    SeatName = "A3",
                    PerformanceId = 3,
                    Price = 300
                },
                new Ticket()
                {
                    SeatName = "B6",
                    PerformanceId = 3,
                    Price = 400
                },
                new Ticket()
                {
                    SeatName = "A4",
                    PerformanceId = 4,
                    Price = 450
                },
                new Ticket()
                {
                    SeatName = "B7",
                    PerformanceId = 4,
                    Price = 550
                },
                new Ticket()
                {
                    SeatName = "A5",
                    PerformanceId = 5,
                    Price = 350
                },
                new Ticket()
                {
                    SeatName = "B8",
                    PerformanceId = 5,
                    Price = 450
                },
                new Ticket()
                {
                    SeatName = "A6",
                    PerformanceId = 6,
                    Price = 500
                },
                new Ticket()
                {
                    SeatName = "B9",
                    PerformanceId = 6,
                    Price = 600
                }
            };
            #endregion

            #region TicketBooking
            var bookings = new List<TicketBooking>()
            {
                new TicketBooking()
                {
                    CustomerId = 1,
                    TicketId = 1,
                    OperationDate = DateTime.Parse("2026-10-20T12:15:00"),
                    IsSold = true
                },
                new TicketBooking()
                {
                    CustomerId = 2,
                    TicketId = 3,
                    OperationDate = DateTime.Parse("2026-10-21T09:40:00"),
                    IsSold = true
                },
                new TicketBooking()
                {
                    CustomerId = 3,
                    TicketId = 4,
                    OperationDate = DateTime.Parse("2026-10-22T16:05:00"),
                    IsSold = false
                },
                new TicketBooking()
                {
                    CustomerId = 4,
                    TicketId = 6,
                    OperationDate = DateTime.Parse("2026-10-23T11:20:00"),
                    IsSold = true
                },
                new TicketBooking()
                {
                    CustomerId = 1,
                    TicketId = 8,
                    OperationDate = DateTime.Parse("2026-10-25T18:45:00"),
                    IsSold = false
                },
                new TicketBooking()
                {
                    CustomerId = 2,
                    TicketId = 11,
                    OperationDate = DateTime.Parse("2026-10-27T14:10:00"),
                    IsSold = true
                }
            };
            #endregion

            #endregion

            context.AddRange(people);
            context.AddRange(authors);
            context.AddRange(customers);
            context.AddRange(genres);
            context.AddRange(performances);
            context.AddRange(tickets);
            context.AddRange(bookings);

            context.SaveChanges();
        }
    }
}
