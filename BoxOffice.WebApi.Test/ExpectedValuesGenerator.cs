using BoxOffice.Business.Models;
using BoxOffice.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public static class ExpectedValuesGenerator
    {
        public static IEnumerable<AuthorModel> GetExpectedAuthors()
        {
            return new List<AuthorModel>()
            {
                new AuthorModel()
                {
                    Id = 1,
                    FirstName = "William",
                    LastName = "Shakespeare",
                    BirthDate = DateTime.Parse("1985-04-23T00:00:00"),
                    Email = "williamshakespeare@gmail.com",
                    PerformanceIds = new List<int>()
                    {
                        1, 5
                    }
                },
                new AuthorModel()
                {
                    Id = 2,
                    FirstName = "Anton",
                    LastName = "Bulba",
                    BirthDate = DateTime.Parse("1990-01-29T00:00:00"),
                    Email = "antonbulba@gmail.com",
                    PerformanceIds = new List<int>()
                    {
                        2
                    }
                },
                new AuthorModel()
                {
                    Id = 3,
                    FirstName = "Oscar",
                    LastName = "Wilde",
                    BirthDate = DateTime.Parse("1982-10-16T00:00:00"),
                    Email = "oscarwilde@gmail.com",
                    PerformanceIds = new List<int>()
                    {
                        3
                    }
                },
                new AuthorModel()
                {
                    Id = 4,
                    FirstName = "Arthur",
                    LastName = "Miller",
                    BirthDate = DateTime.Parse("1978-10-17T00:00:00"),
                    Email = "arthurmiller@gmail.com",
                    PerformanceIds = new List<int>()
                    {
                        4, 6
                    }
                },
            };
        }

        public static IEnumerable<CustomerModel> GetExpectedCustomers()
        {
            return new List<CustomerModel>()
            {
                new CustomerModel()
                {
                    Id = 1,
                    FirstName = "Emma",
                    LastName = "Johnson",
                    BirthDate = DateTime.Parse("1995-05-14T00:00:00"),
                    Email = "emmajohnson@gmail.com",
                    TicketBookingIds = new List<int>()
                    {
                        1, 5
                    }
                },
                new CustomerModel()
                {
                    Id = 2,
                    FirstName = "James",
                    LastName = "Smith",
                    BirthDate = DateTime.Parse("1988-11-02T00:00:00"),
                    Email = "jamessmith@gmail.com",
                    TicketBookingIds = new List<int>()
                    {
                        2, 6
                    }
                },
                new CustomerModel()
                {
                    Id = 3,
                    FirstName = "Olivia",
                    LastName = "Brown",
                    BirthDate = DateTime.Parse("2000-01-30T00:00:00"),
                    Email = "oliviabrown@gmail.com",
                    TicketBookingIds = new List<int>()
                    {
                        3
                    }
                },
                new CustomerModel()
                {
                    Id = 4,
                    FirstName = "Daniel",
                    LastName = "Taylor",
                    BirthDate = DateTime.Parse("1992-07-21T00:00:00"),
                    Email = "danieltaylor@gmail.com",
                    TicketBookingIds = new List<int>()
                    {
                        4
                    }
                },
            };
        }

        public static IEnumerable<GenreModel> GetExpectedGenres()
        {
            return new List<GenreModel>()
            {
                new GenreModel()
                {
                    Id = 1,
                    GenreName = "Drama",
                    PerformanceIds = new List<int>()
                    {
                        2, 4 
                    }
                },
                new GenreModel()
                {
                    Id = 2,
                    GenreName = "Comedy",
                    PerformanceIds = new List<int>()
                    {
                        3, 5
                    }
                },
                new GenreModel()
                {
                    Id = 3,
                    GenreName = "Musical",
                    PerformanceIds = new List<int>()
                    {
                    }
                },
                new GenreModel()
                {
                    Id = 4,
                    GenreName = "Tragedy",
                    PerformanceIds = new List<int>()
                    {
                        1, 6
                    }
                },
            };
        }

        public static IEnumerable<PerformanceModel> GetExpectedPerformances()
        {
            return new List<PerformanceModel>()
            {
                new PerformanceModel()
                {
                    Id = 1,
                    PerformanceName = "Hamlet",
                    AuthorId = 1,
                    GenreId = 4,
                    EventDate = DateTime.Parse("2026-11-05T19:00:00"),
                    TicketIds = new List<int>()
                    {
                        1, 2
                    }
                },
                new PerformanceModel()
                {
                    Id = 2,
                    PerformanceName = "The Cherry Orchard",
                    AuthorId = 2,
                    GenreId = 1,
                    EventDate = DateTime.Parse("2026-11-12T18:30:00"),
                    TicketIds = new List<int>()
                    {
                        3, 4
                    }
                },
                new PerformanceModel()
                {
                    Id = 3,
                    PerformanceName = "The Importance of Being Earnest",
                    AuthorId = 3,
                    GenreId = 2,
                    EventDate = DateTime.Parse("2026-11-20T19:00:00"),
                    TicketIds = new List<int>()
                    {
                        5, 6
                    }
                },
                new PerformanceModel()
                {
                    Id = 4,
                    PerformanceName = "Death of a Salesman",
                    AuthorId = 4,
                    GenreId = 1,
                    EventDate = DateTime.Parse("2026-12-01T19:30:00"),
                    TicketIds = new List<int>()
                    {
                        7, 8
                    }
                },
                new PerformanceModel()
                {
                    Id = 5,
                    PerformanceName = "A Midsummer Night's Dream",
                    AuthorId = 1,
                    GenreId = 2,
                    EventDate = DateTime.Parse("2026-12-10T18:00:00"),
                    TicketIds = new List<int>()
                    {
                        9, 10
                    }
                },
                new PerformanceModel()
                {
                    Id = 6,
                    PerformanceName = "The Crucible",
                    AuthorId = 4,
                    GenreId = 4,
                    EventDate = DateTime.Parse("2026-12-18T19:00:00"),
                    TicketIds = new List<int>()
                    {
                        11, 12
                    }
                }
            };
        }

        public static IEnumerable<TicketModel> GetExpectedTickets()
        {
            return new List<TicketModel>()
            {
                new TicketModel()
                {
                    Id = 1,
                    SeatName = "A1",
                    PerformanceId = 1,
                    Price = 250,
                    TicketBookingId = 1
                },
                new TicketModel()
                {
                    Id = 2,
                    SeatName = "B4",
                    PerformanceId = 1,
                    Price = 350,
                    TicketBookingId = null
                },
                new TicketModel()
                {
                    Id = 3,
                    SeatName = "A2",
                    PerformanceId = 2,
                    Price = 400,
                    TicketBookingId = 2
                },
                new TicketModel()
                {
                    Id = 4,
                    SeatName = "B5",
                    PerformanceId = 2,
                    Price = 500,
                    TicketBookingId = 3
                },
                new TicketModel()
                {
                    Id = 5,
                    SeatName = "A3",
                    PerformanceId = 3,
                    Price = 300,
                    TicketBookingId = null
                },
                new TicketModel()
                {
                    Id = 6,
                    SeatName = "B6",
                    PerformanceId = 3,
                    Price = 400,
                    TicketBookingId = 4
                },
                new TicketModel()
                {
                    Id = 7,
                    SeatName = "A4",
                    PerformanceId = 4,
                    Price = 450,
                    TicketBookingId = null
                },
                new TicketModel()
                {
                    Id = 8,
                    SeatName = "B7",
                    PerformanceId = 4,
                    Price = 550,
                    TicketBookingId = 5
                },
                new TicketModel()
                {
                    Id = 9,
                    SeatName = "A5",
                    PerformanceId = 5,
                    Price = 350,
                    TicketBookingId = null
                },
                new TicketModel()
                {
                    Id = 10,
                    SeatName = "B8",
                    PerformanceId = 5,
                    Price = 450,
                    TicketBookingId = null
                },
                new TicketModel()
                {
                    Id = 11,
                    SeatName = "A6",
                    PerformanceId = 6,
                    Price = 500,
                    TicketBookingId = 6
                },
                new TicketModel()
                {
                    Id = 12,
                    SeatName = "B9",
                    PerformanceId = 6,
                    Price = 600,
                    TicketBookingId = null
                }
            };
        }

        public static IEnumerable<TicketBookingModel> GetExpectedTicketBookings()
        {
            return new List<TicketBookingModel>()
            {
                new TicketBookingModel()
                {
                    Id = 1,
                    CustomerId = 1,
                    TicketId = 1,
                    OperationDate = DateTime.Parse("2026-10-20T12:15:00"),
                    IsSold = true
                },
                new TicketBookingModel()
                {
                    Id = 2,
                    CustomerId = 2,
                    TicketId = 3,
                    OperationDate = DateTime.Parse("2026-10-21T09:40:00"),
                    IsSold = true
                },
                new TicketBookingModel()
                {
                    Id = 3,
                    CustomerId = 3,
                    TicketId = 4,
                    OperationDate = DateTime.Parse("2026-10-22T16:05:00"),
                    IsSold = false
                },
                new TicketBookingModel()
                {
                    Id = 4,
                    CustomerId = 4,
                    TicketId = 6,
                    OperationDate = DateTime.Parse("2026-10-23T11:20:00"),
                    IsSold = true
                },
                new TicketBookingModel()
                {
                    Id = 5,
                    CustomerId = 1,
                    TicketId = 8,
                    OperationDate = DateTime.Parse("2026-10-25T18:45:00"),
                    IsSold = false
                },
                new TicketBookingModel()
                {
                    Id = 6,
                    CustomerId = 2,
                    TicketId = 11,
                    OperationDate = DateTime.Parse("2026-10-27T14:10:00"),
                    IsSold = true
                }
            };
        }

        public static IEnumerable<AuthorActivityModel> GetExpectedAuthorActivityModel()
        {
            return new List<AuthorActivityModel>()
            {
                new AuthorActivityModel()
                {
                    AuthorId = 4,
                    FullName = "Arthur Miller",
                    IncomeSum = 500,
                    BestPerformanceId = 6
                },
                new AuthorActivityModel()
                {
                    AuthorId = 2,
                    FullName = "Anton Bulba",
                    IncomeSum = 400,
                    BestPerformanceId = 2
                },
                new AuthorActivityModel()
                {
                    AuthorId = 3,
                    FullName = "Oscar Wilde",
                    IncomeSum = 400,
                    BestPerformanceId = 3
                }
            };
        }

        public static IEnumerable<CustomerActivityModel> GetExpectedCustomerActivityModel()
        {
            return new List<CustomerActivityModel>()
            {
                new CustomerActivityModel()
                {
                    CustomerId = 2,
                    FullName = "James Smith",
                    SpendsOnTickets = 900
                },
                new CustomerActivityModel()
                {
                    CustomerId = 1,
                    FullName = "Emma Johnson",
                    SpendsOnTickets = 800
                },
                new CustomerActivityModel()
                {
                    CustomerId = 3,
                    FullName = "Olivia Brown",
                    SpendsOnTickets = 500
                }
            };
        }
    }
}
