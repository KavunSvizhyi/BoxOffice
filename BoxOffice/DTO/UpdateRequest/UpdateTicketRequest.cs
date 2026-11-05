using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdateTicketRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string SeatName { get; set; } = string.Empty;

        [Required]
        public int PerformanceId { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
