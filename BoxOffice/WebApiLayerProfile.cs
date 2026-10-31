using AutoMapper;
using BoxOffice.Business.Models;
using BoxOffice.Data.Entities;
using BoxOffice.DTO.AddRequest;
using BoxOffice.DTO.PostResponse;
using BoxOffice.DTO.UpdateRequest;

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

            CreateMap<UpdatePersonRequest, CustomerModel>();
            CreateMap<UpdatePersonRequest, AuthorModel>();
            CreateMap<UpdateGenreRequest, GenreModel>();
            CreateMap<UpdatePerformanceRequest, PerformanceModel>();
            CreateMap<UpdateTicketRequest, TicketModel>();

            CreateMap<AuthorModel, PostPersonResponse>();
            CreateMap<GenreModel, PostGenreResponse>();
            CreateMap<PerformanceModel, PostPerformanceResponse>();
            CreateMap<TicketModel, PostTicketResponse>();
        }
    }
}
