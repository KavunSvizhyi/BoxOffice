using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.AddRequest
{
    public class AddTicketBookingRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TicketId { get; set; }
    }
}
