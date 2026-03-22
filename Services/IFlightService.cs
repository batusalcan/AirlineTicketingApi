using AirlineTicketingApi.DTOs;

namespace AirlineTicketingApi.Services
{
  public interface IFlightService
  {
        // Adds a single flight to the database
        Task<StatusResponseDto> AddFlightAsync(AddFlightRequestDto request);
        
        // Queries flights based on user parameters and returns paginated results
        Task<IEnumerable<FlightResponseDto>> QueryFlightsAsync(QueryFlightRequestDto request, int pageNumber, int pageSize);

        // Processes a file stream and adds multiple flights
        Task<StatusResponseDto> AddFlightsFromFileAsync(Stream fileStream);
  }
}