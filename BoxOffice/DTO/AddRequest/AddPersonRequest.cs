using System.ComponentModel.DataAnnotations;

namespace BoxOffice.DTO.AddRequest
{
    public class AddPersonRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
