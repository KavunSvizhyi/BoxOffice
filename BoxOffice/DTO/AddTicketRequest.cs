namespace BoxOffice.DTO
{
    public class AddTicketRequest
    {
        public string SeatName { get; set; } = null!;

        public int PerformanceId { get; set; }

        public decimal Price { get; set; }
    }
}
