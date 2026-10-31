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
    public class PerformanceService : IPerformanceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PerformanceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PerformanceModel> AddAsync(PerformanceModel model)
        {
            await ValidateAsync(model);

            var performance = _mapper.Map<Performance>(model);
            await _unitOfWork.PerformanceRepository.AddAsync(performance);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PerformanceModel>(performance);
        }

        public async Task DeleteAsync(int id)
        {
            var performance = await ValidateDeletionAsync(id);

            _unitOfWork.PerformanceRepository.Delete(performance!);
            await _unitOfWork.SaveAsync();
        }

        private async Task<Performance?> ValidateDeletionAsync(int id)
        {
            var performance = await _unitOfWork.PerformanceRepository.GetByIdWithDetailsAsync(id);

            if (performance is null)
            {
                throw new TheatreException($"There is no such ID as {id}!");
            }

            if (performance.Tickets.Count > 0)
            {
                throw new TheatreException($"Cannot delete the performance until tickets to it exist (ID = {id})!");
            }

            return performance;
        }

        public async Task<IEnumerable<PerformanceModel>> GetAllAsync()
        {
            var performances = await _unitOfWork.PerformanceRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<PerformanceModel>>(performances);
        }

        public async Task<IEnumerable<PerformanceModel>> GetAllAvailablePerformancesAsync()
        {
            var performances = (await _unitOfWork.PerformanceRepository.GetAllAsync())
                .Where(p => p.EventDate > DateTime.Now);

            return _mapper.Map<IEnumerable<PerformanceModel>>(performances);
        }

        public async Task<IEnumerable<PerformanceModel>> GetByFilterAsync(SearchFilterModel filter)
        {
            var performances = await _unitOfWork.PerformanceRepository.GetAllAsync();

            if (filter.PerformanceName is not null)
            {
                performances = performances
                    .Where(p => p.PerformanceName
                        .Contains(filter.PerformanceName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (filter.GenreId is not null)
            {
                performances = performances
                    .Where(p => p.GenreId == filter.GenreId);
            }

            if (filter.AuthorId is not null)
            {
                performances = performances
                    .Where(p => p.AuthorId == filter.AuthorId);
            }

            if (filter.StartDate is not null)
            {
                performances = performances
                    .Where(p => p.EventDate >= filter.StartDate);
            }

            if (filter.EndDate is not null)
            {
                performances = performances
                    .Where(p => p.EventDate <= filter.EndDate);
            }

            return _mapper.Map<IEnumerable<PerformanceModel>>(performances);
        }

        public async Task<PerformanceModel?> GetByIdAsync(int id)
        {
            var performance = await _unitOfWork.PerformanceRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<PerformanceModel>(performance);
        }

        public async Task UpdateAsync(PerformanceModel model)
        {
            await ValidateAsync(model);

            var performance = await _unitOfWork.PerformanceRepository.GetByIdWithDetailsAsync(model.Id);
            if (performance is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            performance.PerformanceName = model.PerformanceName;
            performance.EventDate = model.EventDate;
            performance.AuthorId = model.AuthorId;
            performance.GenreId = model.GenreId;

            _unitOfWork.PerformanceRepository.Update(performance);
            await _unitOfWork.SaveAsync();
        }

        private async Task ValidateAsync(PerformanceModel model)
        {
            if (string.IsNullOrEmpty(model.PerformanceName) ||
                string.IsNullOrWhiteSpace(model.PerformanceName))
            {
                throw new TheatreException($"{nameof(model.PerformanceName)} cannot be blank!");
            }

            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(model.GenreId);
            var author = await _unitOfWork.AuthorRepository.GetByIdAsync(model.AuthorId);

            if (genre is null)
            {
                throw new TheatreException($"Genre with this {nameof(model.GenreId)} doesn't exist!");
            }

            if (author is null)
            {
                throw new TheatreException($"Author with this {nameof(model.AuthorId)} doesn't exist!");
            }
        }
    }
}
