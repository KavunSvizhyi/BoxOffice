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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BoxOffice.Business.Test
{
    public class PerformanceServiceTest
    {
        private readonly IPerformanceService _performanceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public PerformanceServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _performanceService = new PerformanceService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection()
        {
            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(new List<Performance>());

            var actual = await _performanceService.GetAllAsync();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsExpectedValues()
        {
            IEnumerable<Performance> performances = _fixture.Build<Performance>()
                                                .With(x => x.Tickets, new List<Ticket>())
                                                .Without(x => x.Genre)
                                                .Without(x => x.Author)
                                                .CreateMany();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(performances);

            var expected = _mapper.Map<IEnumerable<PerformanceModel>>(performances);
            var actual = await _performanceService.GetAllAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsNull()
        {
            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            var actual = await _performanceService.GetByIdAsync(5);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedValue()
        {
            var performance = _fixture.Build<Performance>()
                                .With(x => x.Tickets, new List<Ticket>())
                                .Without(x => x.Author)
                                .Without(x => x.Genre)
                                .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(performance);

            var actual = await _performanceService.GetByIdAsync(performance.Id);
            var expected = _mapper.Map<PerformanceModel>(performance);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async Task AddAsync_ReturnsExpectedModel()
        {
            var expected = _fixture.Build<PerformanceModel>()
                                   .With(x => x.TicketIds, new List<int>())
                                   .With(x => x.EventDate, DateTime.Now.AddDays(7))
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.AddAsync(It.IsAny<Performance>()));

            var actual = await _performanceService.AddAsync(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyPerformanceName(string name)
        {
            var data = _fixture.Build<PerformanceModel>()
                               .With(d => d.PerformanceName, name)
                               .With(x => x.EventDate, DateTime.Now.AddDays(7))
                               .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _performanceService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_GenreDoesNotExist()
        {
            var data = _fixture.Build<PerformanceModel>()
                                   .With(x => x.EventDate, DateTime.Now.AddDays(7))
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _performanceService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_AuthorDoesNotExist()
        {
            var data = _fixture.Build<PerformanceModel>()
                                   .With(x => x.EventDate, DateTime.Now.AddDays(7))
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            Func<Task> actual = async () => await _performanceService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidObject()
        {
            var data = _fixture.Build<Performance>()
                               .With(x => x.Tickets, new List<Ticket>())
                               .Without(x => x.Author)
                               .Without(x => x.Genre)
                               .Create();
            var model = _mapper.Map<PerformanceModel>(data);

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _performanceService.UpdateAsync(model);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidId()
        {
            var data = _fixture.Build<PerformanceModel>()
                               .With(x => x.EventDate, DateTime.Now.AddDays(7))
                               .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            Func<Task> actual = async () => await _performanceService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyPerformanceName(string name)
        {
            var data = _fixture.Build<PerformanceModel>()
                               .With(d => d.PerformanceName, name)
                               .With(x => x.EventDate, DateTime.Now.AddDays(7))
                               .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            Func<Task> actual = async () => await _performanceService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_GenreDoesNotExist()
        {
            var data = _fixture.Build<PerformanceModel>()
                                   .With(x => x.EventDate, DateTime.Now.AddDays(7))
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _performanceService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_AuthorDoesNotExist()
        {
            var data = _fixture.Build<PerformanceModel>()
                                   .With(x => x.EventDate, DateTime.Now.AddDays(7))
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            Func<Task> actual = async () => await _performanceService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ValidObject()
        {
            var data = _fixture.Build<Performance>()
                               .Without(x => x.Author)
                               .Without(x => x.Genre)
                               .With(x => x.Tickets, new List<Ticket>())
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _performanceService.DeleteAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task DeleteAsync_InvalidId()
        {
            int testId = _fixture.Create<int>();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Performance);

            Func<Task> actual = async () => await _performanceService.DeleteAsync(testId);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task DeleteAsync_TicketsAreAvailable()
        {
            var data = _fixture.Build<Performance>()
                               .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                             .Without(x => x.TicketBooking)
                                                             .Without(x => x.Performance)
                                                             .CreateMany().ToList())
                               .Without(x => x.Genre)
                               .Without(x => x.Author)
                               .Create();

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _performanceService.DeleteAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region GetByFilterAsync
        [Fact]
        public async Task GetByFilterAsync_ProperReturnValue()
        {
            var expectedRemaining = new Performance()
            {
                Id = 1,
                PerformanceName = "Test1",
                AuthorId = 1,
                GenreId = 1,
                EventDate = DateTime.Now
            };

            var performances = new List<Performance>()
            {
                expectedRemaining,
                new Performance()
                {
                    Id = 2,
                    PerformanceName = "Test2",
                    AuthorId = 1,
                    GenreId = 1,
                    EventDate = DateTime.Now.AddDays(7)
                },
                new Performance()
                {
                    Id = 3,
                    PerformanceName = "Test3",
                    AuthorId = 1,
                    GenreId = 1,
                    EventDate = DateTime.Now.Subtract(TimeSpan.FromDays(7))
                },
                new Performance()
                {
                    Id = 4,
                    PerformanceName = "Test4",
                    AuthorId = 2,
                    GenreId = 1,
                    EventDate = DateTime.Now
                },
                new Performance()
                {
                    Id = 5,
                    PerformanceName = "Test5",
                    AuthorId = 1,
                    GenreId = 2,
                    EventDate = DateTime.Now
                },
                new Performance()
                {
                    Id = 6,
                    PerformanceName = "Tlest6",
                    AuthorId = 1,
                    GenreId = 1,
                    EventDate = DateTime.Now
                }
            };

            var filter = new SearchFilterModel()
            {
                PerformanceName = "test",
                AuthorId = 1,
                GenreId = 1,
                StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                EndDate = DateTime.Now.AddDays(6)
            };

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(performances);

            var expected = _mapper.Map<IEnumerable<PerformanceModel>>(new List<Performance>()
            {
                expectedRemaining
            });
            var actual = await _performanceService.GetByFilterAsync(filter);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetAllAvailablePerformancesAsync
        [Fact]
        public async Task GetAllAvailablePerformancesAsync_ProperReturnValue()
        {
            var expectedRemaining = new List<Performance>()
            {
                _fixture.Build<Performance>()
                        .With(x => x.EventDate, DateTime.Now.AddDays(7))
                        .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                      .Without(x => x.Performance)
                                                      .Without(x => x.TicketBooking)
                                                      .CreateMany().ToList())
                        .Without(x => x.Genre)
                        .Without(x => x.Author)
                        .Create(),

                _fixture.Build<Performance>()
                        .With(x => x.EventDate, DateTime.Now.AddDays(5))
                        .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                      .Without(x => x.Performance)
                                                      .Without(x => x.TicketBooking)
                                                      .CreateMany().ToList())
                        .Without(x => x.Genre)
                        .Without(x => x.Author)
                        .Create()
            };

            var performances = new List<Performance>()
            {
                _fixture.Build<Performance>()
                        .With(x => x.EventDate, DateTime.Now.AddDays(7))
                        .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                      .Without(x => x.Performance)
                                                      .With(x => x.TicketBooking, _fixture.Build<TicketBooking>()
                                                                                          .Without(x => x.Ticket)
                                                                                          .Without(x => x.Customer)
                                                                                          .Create())
                                                      .CreateMany().ToList())
                        .Without(x => x.Genre)
                        .Without(x => x.Author)
                        .Create(),

                _fixture.Build<Performance>()
                        .With(x => x.EventDate, DateTime.Now.Subtract(TimeSpan.FromDays(7)))
                        .With(x => x.Tickets, _fixture.Build<Ticket>()
                                                      .Without(x => x.Performance)
                                                      .Without(x => x.TicketBooking)
                                                      .CreateMany().ToList())
                        .Without(x => x.Genre)
                        .Without(x => x.Author)
                        .Create(),
            };

            performances.AddRange(expectedRemaining);

            _unitOfWorkMock.Setup(u => u.PerformanceRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(performances);

            var expected = _mapper.Map<IEnumerable<PerformanceModel>>(expectedRemaining);
            var actual = await _performanceService.GetAllAvailablePerformancesAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion
    }
}
