namespace BoxOffice.DTO
{
    public class AddTicketBookingRequest
    {
        public int CustomerId { get; set; }

        public int TicketId { get; set; }

        public DateTime OperationDate { get; set; }

        public bool IsSold { get; set; }
    }
}
