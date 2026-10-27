using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class TicketBookingModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int TicketId { get; set; }

        public DateTime OperationDate { get; set; }

        public bool IsSold { get; set; }
    }
}
