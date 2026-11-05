using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdatePerformanceRequest
    {
        [Required]
        public int Id { get; set; }

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
