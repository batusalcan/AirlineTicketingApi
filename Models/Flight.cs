using System.ComponentModel.DataAnnotations;

namespace AirlineTicketingApi.Models
{
  public class Flight
  {
    public int Id {get; set;}
    public string FlightNumber{get; set;} = string.Empty;
    public DateTime DateFrom{get; set;}
    public DateTime DateTo{get; set;}
    public string AirportFrom{get; set;} = string.Empty;
    public string AirportTo{get; set;} = string.Empty;
    public int Duration{get; set;} 

    [ConcurrencyCheck]
    public int Capacity{get; set;}

    public ICollection<Ticket>? Tickets{get; set;}

  }
}