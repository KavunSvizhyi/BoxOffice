using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class TicketBooking : BaseEntity
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime OperationDate { get; set; }

        public bool IsSold { get; set; }
    }
}
