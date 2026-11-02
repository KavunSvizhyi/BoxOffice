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
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BoxOffice.Business.Test
{
    public class TicketServiceTest
    {
        private readonly ITicketService _ticketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public TicketServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _ticketService = new TicketService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection()
        {
            _unitOfWorkMock.Setup(u => u.TicketRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(new List<Ticket>());

            var actual = await _ticketService.GetAllAsync();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsExpectedValues()
        {
            IEnumerable<Ticket> tickets = _fixture.Build<Ticket>()
                                                .Without(x => x.Performance)
                                                .Without(x => x.TicketBooking)
                                                .CreateMany();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(tickets);

            var expected = _mapper.Map<IEnumerable<TicketModel>>(tickets);
            var actual = await _ticketService.GetAllAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsNull()
        {
            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);

            var actual = await _ticketService.GetByIdAsync(5);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedValue()
        {
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .Without(x => x.TicketBooking)
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(ticket);

            var actual = await _ticketService.GetByIdAsync(ticket.Id);
            var expected = _mapper.Map<TicketModel>(ticket);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async Task AddAsync_ReturnsExpectedModel()
        {
            var expected = _fixture.Build<TicketModel>()
                                   .Without(x => x.TicketBookingId)
                                   .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()));

            var actual = await _ticketService.AddAsync(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptySeatName(string name)
        {
            var data = _fixture.Build<TicketModel>()
                               .With(d => d.SeatName, name)
                               .Without(x => x.TicketBookingId)
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _ticketService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_PerformanceDoesNotExist()
        {
            var data = _fixture.Build<TicketModel>()
                                   .Without(x => x.TicketBookingId)
                                   .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()));

            Func<Task> actual = async () => await _ticketService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_NegativePrice()
        {
            var data = _fixture.Build<TicketModel>()
                                   .With(x => x.Price, int.MinValue)
                                   .Without(x => x.TicketBookingId)
                                   .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()));

            Func<Task> actual = async () => await _ticketService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidObject()
        {
            var data = _fixture.Build<Ticket>()
                               .Without(x => x.Performance)
                               .Without(x => x.TicketBooking)
                               .Create();
            var model = _mapper.Map<TicketModel>(data);

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.UpdateAsync(model);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidId()
        {
            var data = _fixture.Build<TicketModel>()
                               .Without(x => x.TicketBookingId)
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);

            Func<Task> actual = async () => await _ticketService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyTicketName(string name)
        {
            var data = _fixture.Build<TicketModel>()
                               .Without(x => x.TicketBookingId)
                               .With(x => x.SeatName, name)
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            Func<Task> actual = async () => await _ticketService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_PerformanceDoesNotExist()
        {
            var data = _fixture.Build<TicketModel>()
                                   .Without(x => x.TicketBookingId)
                                   .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            Func<Task> actual = async () => await _ticketService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_NegativePrice()
        {
            var data = _fixture.Build<TicketModel>()
                                   .With(x => x.Price, int.MinValue)
                                   .Without(x => x.TicketBookingId)
                                   .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.PerformanceRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _ticketService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ValidObject()
        {
            var data = _fixture.Build<Ticket>()
                               .Without(x => x.Performance)
                               .Without(x => x.TicketBooking)
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.DeleteAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task DeleteAsync_InvalidId()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);

            Func<Task> actual = async () => await _ticketService.DeleteAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task DeleteAsync_AlreadyBooked()
        {
            var data = _fixture.Build<Ticket>()
                               .Without(x => x.Performance)
                               .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                   .Without(x => x.Customer)
                                                                   .Without(x => x.Ticket)
                                                                   .Create())
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.DeleteAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region BookTicketAsync
        [Fact]
        public async Task BookTicketAsync_ReturnsExpectedModel()
        {
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .Without(x => x.TicketBooking)
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
               .ReturnsAsync(ticket);
            _unitOfWorkMock.Setup(u => u.TicketBookingRepository.AddAsync(It.IsAny<TicketBooking>()));
            _unitOfWorkMock.Setup(u => u.CustomerRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var expected = _fixture.Create<TicketBookingModel>();
            var actual = await _ticketService.BookTicketAsync(expected.TicketId, expected.CustomerId);

            Assert.Multiple(
                () => actual.TicketId.Should().Be(expected.TicketId),
                () => actual.CustomerId.Should().Be(expected.CustomerId),
                () => actual.IsSold.Should().BeFalse()
            );
        }

        [Fact]
        public async Task BookTicketAsync_CustomerDoesNotExist()
        {
            var data = _fixture.Create<TicketBookingModel>();
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .Without(x => x.TicketBooking)
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
               .ReturnsAsync(ticket);
            _unitOfWorkMock.Setup(u => u.CustomerRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            Func<Task> actual = async () => await _ticketService.BookTicketAsync(data.TicketId, data.CustomerId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task BookTicketAsync_TicketDoesNotExist()
        {
            var data = _fixture.Create<TicketBookingModel>();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);
            _unitOfWorkMock.Setup(u => u.CustomerRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _ticketService.BookTicketAsync(data.TicketId, data.CustomerId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task BookTicketAsync_AlereadyBooked()
        {
            var data = _fixture.Create<TicketBookingModel>();
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                     .Without(x => x.Customer)
                                                                     .Without(x => x.Ticket)
                                                                     .Create())
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(ticket);
            _unitOfWorkMock.Setup(u => u.CustomerRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _ticketService.BookTicketAsync(data.TicketId, data.CustomerId);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region CancelBookingAsync
        [Fact]
        public async Task CancelBookingAsync_ValidObject()
        {
            var data = _fixture.Build<TicketBooking>()
                               .Without(x => x.Customer)
                               .Without(x => x.Ticket)
                               .With(x => x.IsSold, false)
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketBookingRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.CancelBookingAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task CancelBookingAsync_BookingDoesNotExist()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.TicketBookingRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as TicketBooking);

            Func<Task> actual = async () => await _ticketService.CancelBookingAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task CancelBookingAsync_TicketIsSold()
        {
            var data = _fixture.Build<TicketBooking>()
                               .Without(x => x.Customer)
                               .Without(x => x.Ticket)
                               .With(x => x.IsSold, true)
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketBookingRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.CancelBookingAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region GetBookingDetailsByTicketIdAsync
        [Fact]
        public async Task GetBookingDetailsByTicketIdAsync_ReturnsNull()
        {
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .Without(x => x.TicketBooking)
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(ticket);

            var actual = await _ticketService.GetBookingDetailsByTicketIdAsync(ticket.Id);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetBookingDetailsByTicketIdAsync_ReturnsProperValue()
        {
            var ticket = _fixture.Build<Ticket>()
                                 .Without(x => x.Performance)
                                 .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                     .Without(x => x.Customer)
                                                                     .Without(x => x.Ticket)
                                                                     .Create())
                                 .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(ticket);

            var expected = _mapper.Map<TicketBookingModel>(ticket.TicketBooking);
            var actual = await _ticketService.GetBookingDetailsByTicketIdAsync(ticket.Id);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetBookingDetailsByTicketIdAsync_TicketDoesNotExist()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);

            Func<Task> actual = async () => await _ticketService.GetBookingDetailsByTicketIdAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region ChangeToSoldAsync
        [Fact]
        public async Task ChangeToSoldAsync_ValidObject()
        {
            var data = _fixture.Build<Ticket>()
                               .With(x => x.Performance, _fixture.Build<Performance>()
                                                                 .Without(x => x.Author)
                                                                 .Without(x => x.Genre)
                                                                 .With(x => x.Tickets, new List<Ticket>())
                                                                 .With(x => x.EventDate, DateTime.Now.AddDays(5))
                                                                 .Create())
                               .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                   .Without(x => x.Customer)
                                                                   .Without(x => x.Ticket)
                                                                   .With(x => x.IsSold, false)
                                                                   .Create())
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.ChangeToSoldAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task ChangeToSoldAsync_TicketDoesNotExist()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Ticket);

            Func<Task> actual = async () => await _ticketService.ChangeToSoldAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task ChangeToSoldAsync_PerformanceAlreadyHappened()
        {
            var data = _fixture.Build<Ticket>()
                               .With(x => x.Performance, _fixture.Build<Performance>()
                                                                 .Without(x => x.Author)
                                                                 .Without(x => x.Genre)
                                                                 .With(x => x.Tickets, new List<Ticket>())
                                                                 .With(x => x.EventDate, DateTime.Parse("2007-01-01"))
                                                                 .Create())
                               .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                   .Without(x => x.Customer)
                                                                   .Without(x => x.Ticket)
                                                                   .With(x => x.IsSold, false)
                                                                   .Create())
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.ChangeToSoldAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task ChangeToSoldAsync_TickedIsNotBooked()
        {
            var data = _fixture.Build<Ticket>()
                               .With(x => x.Performance, _fixture.Build<Performance>()
                                                                 .Without(x => x.Author)
                                                                 .Without(x => x.Genre)
                                                                 .With(x => x.Tickets, new List<Ticket>())
                                                                 .With(x => x.EventDate, DateTime.Now.AddDays(5))
                                                                 .Create())
                               .Without(x => x.TicketBooking)
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.ChangeToSoldAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task ChangeToSoldAsync_TickedIsSoldAlready()
        {
            var data = _fixture.Build<Ticket>()
                               .With(x => x.Performance, _fixture.Build<Performance>()
                                                                 .Without(x => x.Author)
                                                                 .Without(x => x.Genre)
                                                                 .With(x => x.Tickets, new List<Ticket>())
                                                                 .With(x => x.EventDate, DateTime.Now.AddDays(5))
                                                                 .Create())
                               .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                   .Without(x => x.Customer)
                                                                   .Without(x => x.Ticket)
                                                                   .With(x => x.IsSold, true)
                                                                   .Create())
                               .Create();

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _ticketService.ChangeToSoldAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region GetBookingsByCustomerIdAsync
        [Fact]
        public async Task GetBookingsByCustomerIdAsync_ReturnsProperValues()
        {
            var data = _fixture.Build<Customer>()
                               .Without(x => x.Person)
                               .With(x => x.TicketBookings, _fixture.Build<TicketBooking>()
                                                                    .Without(x => x.Customer)
                                                                    .Without(x => x.Ticket)
                                                                    .CreateMany().ToList())
                               .Create();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            var expected = _mapper.Map<IEnumerable<TicketBookingModel>>(data.TicketBookings);
            var actual = await _ticketService.GetBookingsByCustomerIdAsync(data.Id);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetBookingsByCustomerIdAsync_ReturnsEmptyValues()
        {
            var data = _fixture.Build<Customer>()
                               .Without(x => x.Person)
                               .With(x => x.TicketBookings, new List<TicketBooking>())
                               .Create();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            var actual = await _ticketService.GetBookingsByCustomerIdAsync(data.Id);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBookingsByCustomerIdAsync_CustomerDoesNotExist()
        {
            var testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Customer);

            Func<Task> actual = async () => await _ticketService.GetBookingsByCustomerIdAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region GetAvailableTicketsByPerformanceIdAsync
        [Fact]
        public async Task GetAvailableTicketsByPerformanceIdAsync_ReturnsEmptyCollection()
        {
            var data = _fixture.Build<Performance>()
                               .Without(x => x.Genre)
                               .Without(x => x.Author)
                               .With(x => x.EventDate, DateTime.Now.AddDays(5))
                               .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                             .Without(x => x.Performance)
                                                             .Without(x => x.TicketBooking)
                                                             .CreateMany().ToList())
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            var expected = _mapper.Map<IEnumerable<TicketModel>>(data.Tickets);
            var actual = await _ticketService.GetAvailableTicketsByPerformanceIdAsync(data.Id);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAvailableTicketsByPerformanceIdAsync_EmptyListBecauseOfPastEventDate()
        {
            var data = _fixture.Build<Performance>()
                               .Without(x => x.Genre)
                               .Without(x => x.Author)
                               .With(x => x.EventDate, DateTime.Parse("2007-01-01"))
                               .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                             .Without(x => x.Performance)
                                                             .Without(x => x.TicketBooking)
                                                             .CreateMany().ToList())
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            var actual = await _ticketService.GetAvailableTicketsByPerformanceIdAsync(data.Id);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAvailableTicketsByPerformanceIdAsync_EmptyListBecauseTicketsAreBooked()
        {
            var data = _fixture.Build<Performance>()
                               .Without(x => x.Genre)
                               .Without(x => x.Author)
                               .With(x => x.EventDate, DateTime.Parse("2007-01-01"))
                               .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                             .Without(x => x.Performance)
                                                             .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                                                 .Without(x => x.Ticket)
                                                                                                 .Without(x => x.Customer)
                                                                                                 .Create())
                                                             .CreateMany().ToList)
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            var actual = await _ticketService.GetAvailableTicketsByPerformanceIdAsync(data.Id);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAvailableTicketsByPerformanceIdAsync_PerformanceDoesNotExist()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            Func<Task> actual = async () => await _ticketService.GetAvailableTicketsByPerformanceIdAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion
    }
}
