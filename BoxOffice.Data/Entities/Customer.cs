using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Customer : BaseEntity
    {
        public int PersonId { get; set; }

        public Person Person { get; set; } = null!;

        public ICollection<TicketBooking> TicketBookings { get; set; } = null!;
    }
}
