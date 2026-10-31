namespace BoxOffice.DTO.PostResponse
{
    public class PostPerformanceResponse
    {
        public int Id { get; set; }

        public string PerformanceName { get; set; } = null!;

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public DateTime EventDate { get; set; }
    }
}
