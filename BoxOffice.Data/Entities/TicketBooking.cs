using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class TicketBooking : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public DateTime OperationDate { get; set; }
        public bool IsSold { get; set; }
    }
}
