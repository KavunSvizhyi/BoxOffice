using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class TicketModel
    {
        public int Id { get; set; }

        public string SeatName { get; set; } = null!;

        public int PerfomanceId { get; set; }

        public decimal Price { get; set; }

        public int? TicketBookingId { get; set; }
    }
}
