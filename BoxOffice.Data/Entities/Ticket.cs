using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Ticket : BaseEntity
    {
        [StringLength(20)]
        public string SeatName { get; set; } = null!;

        public int PerfomanceId { get; set; }

        public Perfomance Perfomance { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public TicketBooking? TicketBooking { get; set; }
    }
}
