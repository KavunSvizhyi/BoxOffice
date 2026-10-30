namespace BoxOffice.DTO
{
    public class AddPerformanceRequest
    {
        public string PerformanceName { get; set; } = null!;

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public DateTime EventDate { get; set; }
    }
}
