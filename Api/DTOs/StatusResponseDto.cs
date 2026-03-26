namespace AirlineTicketingApi.DTOs
{
  public class StatusResponseDto
  {
    public bool IsSuccess{get; set;}
    public string Message{ get; set;} = string.Empty;
  }
}