namespace BoxOffice.DTO
{
    public class TicketPostResponse
    {
        public int Id { get; set; }

        public string SeatName { get; set; } = null!;

        public int PerformanceId { get; set; }

        public decimal Price { get; set; }
    }
}
