using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthorModel> AddAsync(AuthorModel model)
        {
            Validate(model);

            var person = await _unitOfWork.PersonRepository.GetByEmailAsync(model.Email);
            var author = _mapper.Map<Author>(model);

            if (person is null)
            {
                author.Person = _mapper.Map<Person>(model);
            }
            else
            {
                bool isPersonAlreadyAuthor = await _unitOfWork.AuthorRepository.ExistsByPersonId(person.Id);
                if (isPersonAlreadyAuthor)
                {
                    throw new TheatreException("The author with this email is already in database!");
                }
                author.PersonId = person.Id;
            }

            await _unitOfWork.AuthorRepository.AddAsync(author);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<AuthorModel>(author);
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _unitOfWork.AuthorRepository.GetByIdAsync(id);

            if (author is not null)
            {
                _unitOfWork.AuthorRepository.Delete(author);

                bool isPersonStillRegirested = await _unitOfWork.AuthorRepository.ExistsByPersonId(author.PersonId);
                if (!isPersonStillRegirested)
                {
                    await _unitOfWork.PersonRepository.DeleteByIdAsync(author.PersonId);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<AuthorModel>> GetAllAsync()
        {
            var authors = await _unitOfWork.AuthorRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<AuthorModel>>(authors);
        }

        public async Task<AuthorModel?> GetByIdAsync(int id)
        {
            var author = await _unitOfWork.AuthorRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<AuthorModel>(author);
        }

        public async Task UpdateAsync(AuthorModel model)
        {
            Validate(model);

            bool isEmailExistsInDb = await _unitOfWork.PersonRepository.ExistsByEmailAsync(model.Email);
            if (isEmailExistsInDb)
            {
                throw new TheatreException("This email is already in database!");
            }

            var author = await _unitOfWork.AuthorRepository.GetByIdWithDetailsAsync(model.Id);
            if (author is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            author.Person.FirstName = model.FirstName;
            author.Person.LastName = model.LastName;
            author.Person.BirthDate = model.BirthDate;
            author.Person.Email = model.Email;

            await _unitOfWork.SaveAsync();
        }

        private void Validate(AuthorModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new TheatreException($"{nameof(model.LastName)} cannot be blank!");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new TheatreException($"{nameof(model.FirstName)} cannot be blank!");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new TheatreException($"{nameof(model.Email)} cannot be blank!");
            }

            if (model.BirthDate.Year < 1900 ||
                model.BirthDate > DateTime.Now)
            {
                throw new TheatreException($"{nameof(model.BirthDate)} is out of the range (The year of 1900 to Now)!");
            }
        }
    }
}
