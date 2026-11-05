using AutoFixture;
using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Services;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Data;
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
    public class AuthorServiceTest
    {
        private readonly IAuthorService _authorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IFixture _fixture;

        public AuthorServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWork = _unitOfWorkMock.Object;

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<BusinessLayerProfile>(), NullLoggerFactory.Instance);
            _mapper = configuration.CreateMapper();

            _authorService = new AuthorService(_mapper, _unitOfWork);
            _fixture = new Fixture();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection()
        {
            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(new List<Author>());

            var actual = await _authorService.GetAllAsync();

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsExpectedValues()
        {
            IEnumerable<Author> authors = _fixture.Build<Author>()
                                                  .With(a => a.Performances, new List<Performance>())
                                                  .CreateMany();

            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetAllWithDetailsAsync())
                .ReturnsAsync(authors);

            var expected = _mapper.Map<IEnumerable<AuthorModel>>(authors);
            var actual = await _authorService.GetAllAsync();

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnsNull()
        {
            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Author);

            var actual = await _authorService.GetByIdAsync(5);

            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedValue()
        {
            Author author = _fixture.Build<Author>()
                                     .With(a => a.Performances, new List<Performance>())
                                     .Create();
            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(author);

            var actual = await _authorService.GetByIdAsync(author.Id);
            var expected = _mapper.Map<AuthorModel>(author);

            actual.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async Task AddAsync_ReturnsExpectedModel()
        {
            var expected = _fixture.Build<AuthorModel>()
                                   .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                                   .With(a => a.PerformanceIds, new List<int>())
                                   .Create();

            _unitOfWorkMock.Setup(u => u.AuthorRepository.AddAsync(It.IsAny<Author>()));

            _unitOfWorkMock.Setup(u => u.PersonRepository.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Person);

            var actual = await _authorService.AddAsync(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyFirstName(string name)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.FirstName, name)
                               .Create();

            Func<Task> actual = async () => await _authorService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyLastName(string name)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.LastName, name)
                               .Create();

            Func<Task> actual = async () => await _authorService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddAsync_EmptyEmail(string email)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.Email, email)
                               .Create();

            Func<Task> actual = async () => await _authorService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_InvalidBirthDate()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(d => d.BirthDate, DateTime.Parse("1899-01-01"))
                               .Create();

            Func<Task> actual = async () => await _authorService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_FutureBirthDate()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(d => d.BirthDate, DateTime.Now.AddYears(1))
                               .Create();

            Func<Task> actual = async () => await _authorService.AddAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task AddAsync_AlreadyExisting()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            var person = _fixture.Create<Person>();

            _unitOfWorkMock.Setup(u => u.PersonRepository.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(person);

            _unitOfWorkMock.Setup(u => u.AuthorRepository.ExistsByPersonId(It.IsAny<int>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidObject()
        {
            var data = _fixture.Build<Author>()
                               .With(x => x.Performances, new List<Performance>())
                               .With(x => x.Person, _fixture.Build<Person>()
                                                            .With(x => x.BirthDate, DateTime.Parse("2007-01-01"))
                                                            .Create())
                               .Create();
            var model = _mapper.Map<AuthorModel>(data);

            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _authorService.UpdateAsync(model);

            await actual.Should().NotThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidId()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            _unitOfWorkMock.Setup(u => u.PersonRepository.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Author);

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyFirstName(string name)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.FirstName, name)
                               .Create();

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyLastName(string name)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.LastName, name)
                               .Create();

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_EmptyEmail(string email)
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .With(d => d.Email, email)
                               .Create();

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_InvalidBirthDate()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(d => d.BirthDate, DateTime.Parse("1899-01-01"))
                               .Create();

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_FutureBirthDate()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(d => d.BirthDate, DateTime.Now.AddYears(1))
                               .Create();

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }

        [Fact]
        public async Task UpdateAsync_AlreadyExistingEmail()
        {
            var data = _fixture.Build<AuthorModel>()
                               .With(a => a.BirthDate, DateTime.Parse("2007-01-09"))
                               .Create();

            _unitOfWorkMock.Setup(u => u.PersonRepository.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            Func<Task> actual = async () => await _authorService.UpdateAsync(data);

            await actual.Should().ThrowAsync<TheatreException>();
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ValidObject()
        {
            var data = _fixture.Build<Author>()
                               .With(x => x.Performances, new List<Performance>())
                               .Create();

            _unitOfWorkMock.Setup(u => u.AuthorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(data);

            Func<Task> actual = async () => await _authorService.DeleteAsync(data.Id);

            await actual.Should().NotThrowAsync<TheatreException>();
        }
        #endregion
    }
}
