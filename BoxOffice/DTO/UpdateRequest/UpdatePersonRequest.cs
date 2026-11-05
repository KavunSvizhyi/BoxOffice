using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdatePersonRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }
    }
}
