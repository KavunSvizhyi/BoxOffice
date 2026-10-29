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
    public class PerfomanceService : IPerfomanceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PerfomanceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PerfomanceModel> AddAsync(PerfomanceModel model)
        {
            await ValidateAsync(model);

            var perfomance = _mapper.Map<Perfomance>(model);
            await _unitOfWork.PerfomanceRepository.AddAsync(perfomance);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PerfomanceModel>(perfomance);
        }

        public async Task DeleteAsync(int id)
        {
            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdWithDetailsAsync(id);

            if (perfomance is not null &&
                perfomance.Tickets.Count > 0)
            {
                throw new TheatreException($"Cannot delete the perfomance until a tickets to it exists (ID = {id})!");
            }

            await _unitOfWork.PerfomanceRepository.DeleteByIdAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<PerfomanceModel>> GetAllAsync()
        {
            var perfomances = await _unitOfWork.PerfomanceRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<PerfomanceModel>>(perfomances);
        }

        public async Task<IEnumerable<PerfomanceModel>> GetAllAvailablePerfomancesAsync()
        {
            var perfomances = (await _unitOfWork.PerfomanceRepository.GetAllAsync())
                .Where(p => p.EventDate > DateTime.Now);

            return _mapper.Map<IEnumerable<PerfomanceModel>>(perfomances);
        }

        public async Task<IEnumerable<PerfomanceModel>> GetByFilterAsync(SearchFilterModel filter)
        {
            var perfomances = await _unitOfWork.PerfomanceRepository.GetAllAsync();

            if (filter.PerfomanceName is not null)
            {
                perfomances = perfomances
                    .Where(p => p.PerfomanceName
                        .Contains(filter.PerfomanceName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (filter.GenreId is not null)
            {
                perfomances = perfomances
                    .Where(p => p.GenreId == filter.GenreId);
            }

            if (filter.AuthorId is not null)
            {
                perfomances = perfomances
                    .Where(p => p.AuthorId == filter.AuthorId);
            }

            if (filter.StartDate is not null)
            {
                perfomances = perfomances
                    .Where(p => p.EventDate >= filter.StartDate);
            }

            if (filter.EndDate is not null)
            {
                perfomances = perfomances
                    .Where(p => p.EventDate <= filter.EndDate);
            }

            return _mapper.Map<IEnumerable<PerfomanceModel>>(perfomances);
        }

        public async Task<PerfomanceModel?> GetByIdAsync(int id)
        {
            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<PerfomanceModel>(perfomance);
        }

        public async Task<IEnumerable<PerfomanceModel>> GetPerfomancesByCustomerIdAsync(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(customerId);
            if (customer is null)
            {
                throw new TheatreException($"There is no such ID as {customerId}!");
            }

            var perfomances = customer.TicketBookings.Select(t => t.Ticket.Perfomance);

            return _mapper.Map<IEnumerable<PerfomanceModel>>(perfomances);
        }

        public async Task UpdateAsync(PerfomanceModel model)
        {
            await ValidateAsync(model);

            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdWithDetailsAsync(model.Id);
            if (perfomance is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            perfomance.PerfomanceName = model.PerfomanceName;
            perfomance.EventDate = model.EventDate;
            perfomance.AuthorId = model.AuthorId;
            perfomance.GenreId = model.GenreId;

            _unitOfWork.PerfomanceRepository.Update(perfomance);
            await _unitOfWork.SaveAsync();
        }

        private async Task ValidateAsync(PerfomanceModel model)
        {
            if (string.IsNullOrEmpty(model.PerfomanceName) ||
                string.IsNullOrWhiteSpace(model.PerfomanceName))
            {
                throw new TheatreException($"{nameof(model.PerfomanceName)} cannot be blank!");
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
