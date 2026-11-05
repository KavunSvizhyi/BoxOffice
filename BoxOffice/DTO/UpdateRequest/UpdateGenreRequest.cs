using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdateGenreRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string GenreName { get; set; } = string.Empty;
    }
}
