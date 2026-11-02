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
    public class GenreServiceTest
    {
        private readonly IGenreService _genreService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public GenreServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _genreService = new GenreService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection()
        {
            _unitOfWorkMock.Setup(u => u.GenreRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(new List<Genre>());

            var actual = await _genreService.GetAllAsync();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsExpectedValues()
        {
            IEnumerable<Genre> genres = _fixture.Build<Genre>()
                                                .With(x => x.Performances, new List<Performance>())
                                                .CreateMany();

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(genres);

            var expected = _mapper.Map<IEnumerable<GenreModel>>(genres);
            var actual = await _genreService.GetAllAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsNull()
        {
            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Genre);

            var actual = await _genreService.GetByIdAsync(5);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedValue()
        {
            var genre = _fixture.Build<Genre>()
                                .With(x => x.Performances, new List<Performance>())
                                .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(genre);

            var actual = await _genreService.GetByIdAsync(genre.Id);
            var expected = _mapper.Map<GenreModel>(genre);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async Task AddAsync_ReturnsExpectedModel()
        {
            var expected = _fixture.Build<GenreModel>()
                                   .With(x => x.PerformanceIds, new List<int>())
                                   .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.AddAsync(It.IsAny<Genre>()));

            var actual = await _genreService.AddAsync(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyGenreName(string name)
        {
            var data = _fixture.Build<GenreModel>()
                               .With(d => d.GenreName, name)
                               .Create();

            Func<Task> actual = async () => await _genreService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidObject()
        {
            var data = _fixture.Build<Genre>()
                               .With(x => x.Performances, new List<Performance>())
                               .Create();
            var model = _mapper.Map<GenreModel>(data);

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _genreService.UpdateAsync(model);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidId()
        {
            var data = _fixture.Create<GenreModel>();

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Genre);

            Func<Task> actual = async () => await _genreService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyGenreName(string name)
        {
            var data = _fixture.Build<GenreModel>()
                               .With(d => d.GenreName, name)
                               .Create();

            Func<Task> actual = async () => await _genreService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ValidObject()
        {
            var data = _fixture.Build<Genre>()
                               .With(x => x.Performances, new List<Performance>())
                               .Create();

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _genreService.DeleteAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task DeleteAsync_InvalidId()
        {
            var data = _fixture.Create<GenreModel>();

            _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Genre);

            Func<Task> actual = async () => await _genreService.DeleteAsync(data.Id);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion
    }
}
