namespace BoxOffice.DTO.PostResponse
{
    public class PostTicketResponse
    {
        public int Id { get; set; }

        public string SeatName { get; set; } = null!;

        public int PerformanceId { get; set; }

        public decimal Price { get; set; }
    }
}
