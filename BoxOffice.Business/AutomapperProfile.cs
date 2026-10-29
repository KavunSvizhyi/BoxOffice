using AutoMapper;
using BoxOffice.Business.Models;
using BoxOffice.Data.Entities;
using Microsoft.Data.SqlClient;

namespace BoxOffice.Business
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<AuthorModel, Person>();

            CreateMap<CustomerModel, Person>();

            CreateMap<Author, AuthorModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Person.LastName))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.Person.BirthDate))
                .ForMember(dest => dest.PerformanceIds, opt => opt.MapFrom(src => src.Performances.Select(p => p.Id)))
                .ReverseMap()
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Performances, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore());

            CreateMap<Customer, CustomerModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Person.LastName))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.Person.BirthDate))
                .ForMember(dest => dest.TicketBookingIds, opt => opt.MapFrom(src => src.TicketBookings.Select(p => p.Id)))
                .ReverseMap()
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketBookings, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore());

            CreateMap<Genre, GenreModel>()
                .ForMember(dest => dest.PerformanceIds, opt => opt.MapFrom(src => src.Performances.Select(p => p.Id)))
                .ReverseMap()
                .ForMember(dest => dest.Performances, opt => opt.Ignore());

            CreateMap<Performance, PerformanceModel>()
                .ForMember(dest => dest.TicketIds, opt => opt.MapFrom(src => src.Tickets.Select(t => t.Id)))
                .ReverseMap()
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<Ticket, TicketModel>()
                .ForMember(dest => dest.TicketBookingId, opt => opt.MapFrom(src => 
                    src.TicketBooking == null ? (int?)null : src.TicketBooking.Id))
                .ReverseMap()
                .ForMember(dest => dest.TicketBooking, opt => opt.Ignore());
             
            CreateMap<TicketBooking, TicketBookingModel>()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Ticket, opt => opt.Ignore());
        }
    }
}
