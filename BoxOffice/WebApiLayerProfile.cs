using AutoMapper;
using BoxOffice.Business.Models;
using BoxOffice.Data.Entities;
using BoxOffice.DTO;

namespace BoxOffice
{
    public class WebApiLayerProfile : Profile
    {
        public WebApiLayerProfile()
        {
            CreateMap<AddPersonRequest, CustomerModel>();
            CreateMap<AddPersonRequest, AuthorModel>();
            CreateMap<AddGenreRequest, GenreModel>();
            CreateMap<AddPerformanceRequest, PerformanceModel>();
            CreateMap<AddTicketRequest, TicketModel>();
            CreateMap<AddTicketBookingRequest, TicketBookingModel>();

            CreateMap<CustomerModel, CustomerPostResponse>();
            CreateMap<AuthorModel, AuthorPostResponse>();
            CreateMap<GenreModel, GenrePostResponse>();
            CreateMap<PerformanceModel, PerformancePostResponse>();
            CreateMap<TicketModel, TicketPostResponse>();
        }
    }
}
