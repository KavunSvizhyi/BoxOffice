using BoxOffice.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Interfaces
{
    public interface ITicketService : IModelService<TicketModel>
    {
        Task<TicketBookingModel> BookTicketAsync(int ticketId, int customerId);

        Task CancelBookingAsync(int ticketId);

        Task<TicketBookingModel?> GetBookingDetailsByTicketIdAsync(int ticketId);

        Task ChangeToSoldAsync(int ticketId);

        Task<IEnumerable<TicketBookingModel>> GetBookingsByCustomerIdAsync(int customerId);

        Task<IEnumerable<TicketModel>> GetAvailableTicketsByPerfomanceIdAsync(int perfomanceId);
    }
}
