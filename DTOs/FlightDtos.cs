namespace AirlineTicketingApi.DTOs
{
  public class AddFlightRequestDto
  {
    public string FlightNumber{get; set;} = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string AirportFrom { get; set; } = string.Empty;
    public string AirportTo { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Capacity { get; set; }
  }

  public class QueryFlightRequestDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string AirportFrom { get; set; } = string.Empty;
        public string AirportTo { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public bool IsRoundTrip { get; set; }
    }

    public class FlightResponseDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}