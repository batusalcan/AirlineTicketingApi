using Microsoft.AspNetCore.Authentication;

namespace AirlineTicketingApi.Models
{
  public class User
  {
    public int Id{get; set;}
    public string FullName{get; set;} = string.Empty;
    public string Email{get; set;} = string.Empty;
    public string PasswordHash{get; set;} = string.Empty;
    public string Role{get; set;}= "Passenger";

    public ICollection<Ticket>? Tickets {get; set;}
  }
}