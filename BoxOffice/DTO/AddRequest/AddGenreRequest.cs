using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.AddRequest
{
    public class AddGenreRequest
    {
        [Required]
        public string GenreName { get; set; } = string.Empty;
    }
}
