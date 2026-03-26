namespace AirlineTicketingApi.DTOs
{
    // 1. Used for "Buy Ticket" API input
    public class BuyTicketRequestDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string PassengerName { get; set; } = string.Empty;
    }

    // 2. Used for "Buy Ticket" API output
    public class BuyTicketResponseDto
    {
        public string TransactionStatus { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;
    }

    // 3. Used for "Check in" API input
    public class CheckInRequestDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string PassengerName { get; set; } = string.Empty;
    }

    // 4. Used for "Query Flight Passenger List" API output
    public class PassengerResponseDto
    {
        public string PassengerName { get; set; } = string.Empty;
        public int? SeatNumber { get; set; }
    }
}