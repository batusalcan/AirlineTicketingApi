namespace AirlineTicketingApi.Models
{
  public class Ticket
  {
    public int Id{get; set;}
    public string TicketNumber{get; set;}= string.Empty;

    public int FlightId {get; set;}
    public Flight? Flight{get; set;}

    public int PassengerId{get; set;}
    public User? Passenger{get; set;}

    public int? SeatNumber{get; set;}
  }
}