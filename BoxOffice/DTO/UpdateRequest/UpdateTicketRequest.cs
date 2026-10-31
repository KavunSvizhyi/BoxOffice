namespace BoxOffice.DTO.UpdateRequest
{
    public class UpdateTicketRequest
    {
        public int Id { get; set; }

        public string SeatName { get; set; } = null!;

        public int PerformanceId { get; set; }

        public decimal Price { get; set; }
    }
}
