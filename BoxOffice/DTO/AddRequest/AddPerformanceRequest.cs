using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.AddRequest
{
    public class AddPerformanceRequest
    {
        [Required]
        public string PerformanceName { get; set; } = string.Empty;

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }
    }
}
