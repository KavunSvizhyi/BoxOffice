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
    public class CustomerServiceTest
    {
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public CustomerServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _customerService = new CustomerService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection()
        {
            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(new List<Customer>());

            var actual = await _customerService.GetAllAsync();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsExpectedValues()
        {
            IEnumerable<Customer> customers = _fixture.Build<Customer>()
                                                  .With(a => a.TicketBookings, new List<TicketBooking>())
                                                  .CreateMany();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(customers);

            var expected = _mapper.Map<IEnumerable<CustomerModel>>(customers);
            var actual = await _customerService.GetAllAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsNull()
        {
            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Customer);

            var actual = await _customerService.GetByIdAsync(5);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedValue()
        {
            Customer customer = _fixture.Build<Customer>()
                                     .With(a => a.TicketBookings, new List<TicketBooking>())
                                     .Create();
            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(customer);

            var actual = await _customerService.GetByIdAsync(customer.Id);
            var expected = _mapper.Map<CustomerModel>(customer);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async Task AddAsync_ReturnsExpectedModel()
        {
            var expected = _fixture.Build<CustomerModel>()
                                   .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                                   .With(a => a.TicketBookingIds, new List<int>())
                                   .Create();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.AddAsync(It.IsAny<Customer>()));

            _unitOfWorkMock.Setup(u => u.PersonRepository.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Person);

            var actual = await _customerService.AddAsync(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyFirstName(string name)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.FirstName, name)
                               .Create();

            Func<Task> actual = async () => await _customerService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyLastName(string name)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.LastName, name)
                               .Create();

            Func<Task> actual = async () => await _customerService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyEmail(string email)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.Email, email)
                               .Create();

            Func<Task> actual = async () => await _customerService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_InvalidBirthDate()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(d => d.BirthDate, DateTime.Parse("1899-01-01"))
                               .Create();

            Func<Task> actual = async () => await _customerService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_FutureBirthDate()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(d => d.BirthDate, DateTime.Now.AddYears(1))
                               .Create();

            Func<Task> actual = async () => await _customerService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_AlreadyExisting()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            var person = _fixture.Create<Person>();

            _unitOfWorkMock.Setup(u => u.PersonRepository.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(person);

            _unitOfWorkMock.Setup(u => u.CustomerRepository.ExistsByPersonId(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidObject()
        {
            var data = _fixture.Build<Customer>()
                               .With(x => x.TicketBookings, new List<TicketBooking>())
                               .With(x => x.Person, _fixture.Build<Person>()
                                                            .With(x => x.BirthDate, DateTime.Parse("2007-01-01"))
                                                            .Create())
                               .Create();
            var model = _mapper.Map<CustomerModel>(data);

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _customerService.UpdateAsync(model);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidId()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            _unitOfWorkMock.Setup(u => u.PersonRepository.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Customer);

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyFirstName(string name)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.FirstName, name)
                               .Create();

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyLastName(string name)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.LastName, name)
                               .Create();

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyEmail(string email)
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.Email, email)
                               .Create();

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidBirthDate()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(d => d.BirthDate, DateTime.Parse("1899-01-01"))
                               .Create();

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_FutureBirthDate()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(d => d.BirthDate, DateTime.Now.AddYears(1))
                               .Create();

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_AlreadyExistingEmail()
        {
            var data = _fixture.Build<CustomerModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            _unitOfWorkMock.Setup(u => u.PersonRepository.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _customerService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ValidObject()
        {
            var data = _fixture.Build<Customer>()
                               .With(x => x.TicketBookings, new List<TicketBooking>())
                               .Create();

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _customerService.DeleteAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }
        #endregion

        #region GetCustomersByPerformanceIdAsync
        [Fact]
        public async Task GetCustomersByPerformanceIdAsync_EmptyCollection()
        {
            var performance = _fixture.Build<Performance>()
                                      .Without(x => x.Author)
                                      .With(x => x.Tickets, new List<Ticket>())
                                      .Without(x => x.Genre)
                                      .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(performance);

            var actual = await _customerService.GetCustomersByPerformanceIdAsync(performance.Id);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCustomersByPerformanceIdAsync_ExpectedValues()
        {
            var customerBuilder = _fixture.Build<Customer>()
                                   .With(x => x.TicketBookings, new List<TicketBooking>());
            var bookingBuilder = _fixture.Build<TicketBooking>()
                                  .Without(x => x.Ticket)
                                  .With(x => x.Customer, customerBuilder.Create());
            var ticketBuilder = _fixture.Build<Ticket>()
                                  .Without(x => x.Performance)
                                  .With(x => x.TicketBooking, bookingBuilder.Create());
            var performance = _fixture.Build<Performance>()
                                      .With(x => x.Tickets, ticketBuilder.CreateMany().ToList())
                                      .Without(x => x.Author)
                                      .Without(x => x.Genre)
                                      .Create();
            var result = performance.Tickets.Select(x => x.TicketBooking).Where(x => x != null).Select(x => x!.Customer);

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(performance);

            var expected = _mapper.Map<IEnumerable<CustomerModel>>(result);
            var actual = await _customerService.GetCustomersByPerformanceIdAsync(performance.Id);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetCustomersByPerformanceIdAsync_PerformanceDoesNotExist()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            Func<Task> actual = async () => await _customerService.GetCustomersByPerformanceIdAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion
    }
}
