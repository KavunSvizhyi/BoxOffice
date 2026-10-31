namespace BoxOffice.DTO.PostResponse
{
    public class PostPersonResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string Email { get; set; } = null!;
    }
}
