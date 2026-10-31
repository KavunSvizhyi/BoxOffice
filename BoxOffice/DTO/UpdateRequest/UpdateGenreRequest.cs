namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdateGenreRequest
    {
        public int Id { get; set; }

        public string GenreName { get; set; } = null!;
    }
}
