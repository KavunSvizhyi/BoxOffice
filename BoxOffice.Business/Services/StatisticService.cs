using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StatisticService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetIncomeOfGenreInPeriodAsync(int genreId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException();
            }

            var genre = await _unitOfWork.GenreRepository.GetByIdWithDetailsAsync(genreId);
            if (genre is null)
            {
                throw new TheatreException($"There is no such ID as {genreId}!");
            }

            return genre.Performances
                .SelectMany(p => p.Tickets)
                .Where(t => t.TicketBooking is not null && 
                            t.TicketBooking.IsSold &&
                            t.TicketBooking.OperationDate >= startDate &&
                            t.TicketBooking.OperationDate <= endDate)
                .Sum(t => t.Price);
        }

        public async Task<IEnumerable<AuthorActivityModel>> GetMostSuccessfulAuthorsAsync(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException($"{nameof(count)} can only be a positive number!");
            }

            var performanceGroups = (await _unitOfWork.PerformanceRepository.GetAllWithDetailsAsync())
                .GroupBy(p => new 
                { 
                    p.AuthorId, 
                    FullName = $"{p.Author!.Person.FirstName} {p.Author!.Person.LastName}"
                });

            return performanceGroups.Select(g => new AuthorActivityModel
            {
                AuthorId = g.Key.AuthorId,
                FullName = g.Key.FullName,
                IncomeSum = g.SelectMany(p => p.Tickets)
                 .Where(t => t.TicketBooking != null && t.TicketBooking.IsSold)
                 .Sum(t => t.Price),

                BestPerformanceId = g.OrderByDescending(p => p.Tickets
                                        .Where(t => t.TicketBooking != null && t.TicketBooking.IsSold)
                                        .Sum(t => t.Price))
                         .Select(p => p.Id)
                         .FirstOrDefault()
            }).Take(count);
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException($"{nameof(count)} can only be a positive number!");
            }

            var customerGroups = (await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync())
                .GroupBy(c => new
                {
                    c.Id,
                    FullName = $"{c.Person.FirstName} {c.Person.LastName}"
                });

            return customerGroups.Select(g => new CustomerActivityModel
            {
                CustomerId = g.Key.Id,
                FullName = g.Key.FullName,
                SpendsOnTickets = g.SelectMany(p => p.TicketBookings)
                                   .Sum(t => t.Ticket.Price)
            }).Take(count);
        }
    }
}
