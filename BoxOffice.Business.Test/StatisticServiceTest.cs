using AutoFixture;
using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Services;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Test
{
    public class StatisticServiceTest
    {
        private readonly IStatisticService _statisticService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public StatisticServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _statisticService = new StatisticService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetMostSuccessfulAuthorsAsync
        [Fact]
        public async Task GetMostSuccessfulAuthorsAsync_ReturnProperCollection()
        {
            var perf1 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .With(p => p.Id, 101)
                .With(p => p.AuthorId, 1)
                .With(p => p.Author, _fixture.Build<Author>()
                    .Without(x => x.Performances)
                    .With(a => a.Id, 1)
                    .With(a => a.Person, _fixture.Build<Person>()
                        .With(person => person.FirstName, "John")
                        .With(person => person.LastName, "Doe")
                        .Create())
                    .Create())
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 100m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                                                            .Without(x => x.Ticket)
                                                            .Without(x => x.Customer)
                                                            .With(b => b.IsSold, true).Create())
                        .Create()
                })
                .Create();

            var perf2 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .With(p => p.Id, 102)
                .With(p => p.AuthorId, 1)
                .With(p => p.Author, perf1.Author)
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 300m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                                                            .Without(x => x.Ticket)
                                                            .Without(x => x.Customer)
                                                            .With(b => b.IsSold, true).Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 500m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                                                            .Without(x => x.Ticket)
                                                            .Without(x => x.Customer)
                                                            .With(b => b.IsSold, false).Create())
                        .Create()
                })
                .Create();

            var perf3 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .With(p => p.Id, 103)
                .With(p => p.AuthorId, 2)
                .With(p => p.Author, _fixture.Build<Author>()
                    .Without(x => x.Performances)
                    .With(a => a.Id, 2)
                    .With(a => a.Person, _fixture.Build<Person>()
                        .With(person => person.FirstName, "Jane")
                        .With(person => person.LastName, "Smith")
                        .Create())
                    .Create())
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 500m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                                                            .Without(x => x.Ticket)
                                                            .Without(x => x.Customer)
                                                            .With(b => b.IsSold, true).Create())
                        .Create()
                })
                .Create();

            var perf4 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .With(p => p.Id, 104)
                .With(p => p.AuthorId, 3)
                .With(p => p.Author, _fixture.Build<Author>()
                    .Without(x => x.Performances)
                    .With(a => a.Id, 3)
                    .With(a => a.Person, _fixture.Build<Person>()
                        .With(person => person.FirstName, "Alice")
                        .With(person => person.LastName, "Brown")
                        .Create())
                    .Create())
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 50m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                                                            .Without(x => x.Ticket)
                                                            .Without(x => x.Customer)
                                                            .With(b => b.IsSold, true).Create())
                        .Create()
                })
                .Create();

            var testData = new List<Performance> { perf1, perf2, perf3, perf4 };

            var count = 2;
            var expected = new List<AuthorActivityModel>
            {
                new AuthorActivityModel
                {
                    AuthorId = 2,
                    FullName = "Jane Smith",
                    IncomeSum = 500m,
                    BestPerformanceId = 103
                },
                new AuthorActivityModel
                {
                    AuthorId = 1,
                    FullName = "John Doe",
                    IncomeSum = 400m,
                    BestPerformanceId = 102
                }
            };

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(testData);

            var actual = await _statisticService.GetMostSuccessfulAuthorsAsync(count);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
        #endregion

        #region GetMostValuableCustomersAsync
        [Fact]
        public async Task GetMostValuableCustomersAsync_ReturnProperCollection()
        {
            var customer1 = _fixture.Build<Customer>()
                .With(c => c.Id, 1)
                .With(c => c.Person, _fixture.Build<Person>()
                    .With(p => p.FirstName, "John")
                    .With(p => p.LastName, "Doe")
                    .Create())
                .With(c => c.TicketBookings, new List<TicketBooking>
                {
                    _fixture.Build<TicketBooking>()
                        .Without(x => x.Customer)
                        .With(tb => tb.Ticket, _fixture.Build<Ticket>()
                                                       .Without(x => x.Performance)
                                                       .Without(x => x.TicketBooking)
                                                       .With(t => t.Price, 100m).Create())
                        .Create(),
                    _fixture.Build<TicketBooking>()
                        .Without(x => x.Customer)
                        .With(tb => tb.Ticket, _fixture.Build<Ticket>()
                                                       .Without(x => x.Performance)
                                                       .Without(x => x.TicketBooking)
                                                       .With(t => t.Price, 300m).Create())
                        .Create()
                })
                .Create();

            var customer2 = _fixture.Build<Customer>()
                .With(c => c.Id, 2)
                .With(c => c.Person, _fixture.Build<Person>()
                    .With(p => p.FirstName, "Jane")
                    .With(p => p.LastName, "Smith")
                    .Create())
                .With(c => c.TicketBookings, new List<TicketBooking>
                {
                    _fixture.Build<TicketBooking>()
                        .Without(x => x.Customer)
                        .With(tb => tb.Ticket, _fixture.Build<Ticket>()
                                                       .Without(x => x.Performance)
                                                       .Without(x => x.TicketBooking)
                                                       .With(t => t.Price, 500m).Create())
                        .Create()
                })
                .Create();

            var customer3 = _fixture.Build<Customer>()
                .With(c => c.Id, 3)
                .With(c => c.Person, _fixture.Build<Person>()
                    .With(p => p.FirstName, "Alice")
                    .With(p => p.LastName, "Brown")
                    .Create())
                .With(c => c.TicketBookings, new List<TicketBooking>
                {
                    _fixture.Build<TicketBooking>()
                        .Without(x => x.Customer)
                        .With(tb => tb.Ticket, _fixture.Build<Ticket>()
                                                       .Without(x => x.Performance)
                                                       .Without(x => x.TicketBooking)
                                                       .With(t => t.Price, 50m).Create())
                        .Create()
                })
                .Create();

            var testData = new List<Customer> { customer1, customer2, customer3 };

            var count = 2;
            
            var expected = new List<CustomerActivityModel>
            {
                new CustomerActivityModel
                {
                    CustomerId = 2,
                    FullName = "Jane Smith",
                    SpendsOnTickets = 500m
                },
                new CustomerActivityModel
                {
                    CustomerId = 1,
                    FullName = "John Doe",
                    SpendsOnTickets = 400m
                }
            };

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(testData);

            var actual = await _statisticService.GetMostValuableCustomersAsync(count);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
        #endregion

        #region GetIncomeOfGenreInPeriodAsync
        [Fact]
        public async Task GetMostValuableCustomersAsync_ReturnProperValue()
        {
            var startDate = new DateTime(2026, 1, 1);
            var endDate = new DateTime(2026, 1, 31);
            const int genreId = 1;

            var performance1 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .Without(x => x.Author)
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 100m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, true)
                            .With(tb => tb.OperationDate, new DateTime(2026, 1, 15))
                            .Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 200m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, true)
                            .With(tb => tb.OperationDate, new DateTime(2026, 1, 31))
                            .Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 500m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, false)
                            .With(tb => tb.OperationDate, new DateTime(2026, 1, 15))
                            .Create())
                        .Create()
                })
                .Create();

            var performance2 = _fixture.Build<Performance>()
                .Without(x => x.Genre)
                .Without(x => x.Author)
                .With(p => p.Tickets, new List<Ticket>
                {
                    _fixture.Build<Ticket>()
                        .With(t => t.Price, 150m)
                        .Without(x => x.Performance)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, true)
                            .With(tb => tb.OperationDate, new DateTime(2026, 1, 1))
                            .Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 300m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, true)
                            .With(tb => tb.OperationDate, new DateTime(2025, 12, 31))
                            .Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 400m)
                        .With(t => t.TicketBooking, _fixture.Build<TicketBooking>()
                            .Without(x => x.Customer)
                            .Without(x => x.Ticket)
                            .With(tb => tb.IsSold, true)
                            .With(tb => tb.OperationDate, new DateTime(2026, 2, 1))
                            .Create())
                        .Create(),
                    _fixture.Build<Ticket>()
                        .Without(x => x.Performance)
                        .With(t => t.Price, 250m)
                        .Without(t => t.TicketBooking)
                        .Create()
                })
                .Create();

            var testGenre = _fixture.Build<Genre>()
                .With(g => g.Id, genreId)
                .With(g => g.Performances, new List<Performance> { performance1, performance2 })
                .Create();

            const decimal expected = 450m;

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(testGenre);

            var actual = await _statisticService.GetIncomeOfGenreInPeriodAsync(genreId, startDate, endDate);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task GetMostValuableCustomersAsync_GenreDoesNotExist()
        {
            int genreId = 1;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Genre);

            Func<Task> actual = async () => await _statisticService.GetIncomeOfGenreInPeriodAsync(genreId, startDate, endDate);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion
    }
}
