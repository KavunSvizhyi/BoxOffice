using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface ITicketService : IModelService<TicketModel>
    {
        Task<TicketBookingModel> BookTicketAsync(int id, int customerId);

        Task CancelBookingAsync(int ticketBookingId);

        Task<TicketBookingModel?> GetBookingDetailsByTicketIdAsync(int id);

        Task ChangeToSoldAsync(int id);

        Task<IEnumerable<TicketBookingModel>> GetBookingsByCustomerIdAsync(int customerId);

        Task<IEnumerable<TicketModel>> GetAvailableTicketsByPerfomanceIdAsync(int perfomanceId);
    }
}
