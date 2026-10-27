using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Ticket : BaseEntity
    {
        public string SeatName { get; set; } = null!;
        public int PerfomanceId { get; set; }
        public Perfomance Perfomance { get; set; } = null!;
        public decimal Price { get; set; }
        public TicketBooking? TicketBooking { get; set; }
    }
}
