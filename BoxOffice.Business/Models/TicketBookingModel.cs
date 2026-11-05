using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class TicketBookingModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public DateTime OperationDate { get; set; }

        [Required]
        public bool IsSold { get; set; }
    }
}
