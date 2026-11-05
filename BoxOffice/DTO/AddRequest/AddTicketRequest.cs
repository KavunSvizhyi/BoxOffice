using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.AddRequest
{
    public class AddTicketRequest
    {
        [Required]
        public string SeatName { get; set; } = string.Empty;

        [Required]
        public int PerformanceId { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
