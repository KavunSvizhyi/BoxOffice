using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface ITicketService : IModelService<TicketModel>
    {
        Task<TicketBookingModel> BookTicket(int ticketId, int customerId);

        Task CancelBooking(int ticketId);

        Task<TicketBookingModel?> GetBookingDetails(int ticketId);

        Task<decimal> GetTicketPriceAsync(int ticketId);

        Task ChangeToSoldAsync(int ticketId);

        Task<IEnumerable<TicketBookingModel>> GetBookingDetailsByPeriodAsync(DateTime startDate, DateTime endTime); 
    }
}
