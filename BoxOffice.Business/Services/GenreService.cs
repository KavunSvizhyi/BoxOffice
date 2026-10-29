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
    public class GenreService : IGenreService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GenreService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GenreModel> AddAsync(GenreModel model)
        {
            Validate(model);

            var genre = _mapper.Map<Genre>(model);
            await _unitOfWork.GenreRepository.AddAsync(genre);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<GenreModel>(genre);
        }

        public async Task DeleteAsync(int id)
        {
            bool IsAnyPerfomanceByGenre = 
                (await _unitOfWork.PerfomanceRepository.GetAllAsync())
                    .Any(p => p.GenreId == id);

            if (IsAnyPerfomanceByGenre)
            {
                throw new TheatreException("Cannot delete the genre until a perfomance with it exists!");
            }

            await _unitOfWork.GenreRepository.DeleteByIdAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<GenreModel>> GetAllAsync()
        {
            var genres = await _unitOfWork.GenreRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<GenreModel>>(genres);
        }

        public async Task<GenreModel?> GetByIdAsync(int id)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<GenreModel>(genre);
        }

        public async Task UpdateAsync(GenreModel model)
        {
            Validate(model);

            var genre = await _unitOfWork.GenreRepository.GetByIdWithDetailsAsync(model.Id);
            if (genre is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            genre.GenreName = model.GenreName;

            _unitOfWork.GenreRepository.Update(genre);
            await _unitOfWork.SaveAsync();
        }

        private static void Validate(GenreModel model)
        {
            if (string.IsNullOrEmpty(model.GenreName) ||
                string.IsNullOrWhiteSpace(model.GenreName))
            {
                throw new TheatreException($"{nameof(model.GenreName)} cannot be blank!");
            }
        }
    }
}
