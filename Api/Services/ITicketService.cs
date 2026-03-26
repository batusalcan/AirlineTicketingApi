using AirlineTicketingApi.DTOs;

namespace AirlineTicketingApi.Services
{
    public interface ITicketService
    {
        // Handles the purchase, capacity reduction, and "Sold out" logic
        Task<BuyTicketResponseDto> BuyTicketAsync(BuyTicketRequestDto request);
        
        // Assigns a simple seat number to a passenger
        Task<StatusResponseDto> CheckInAsync(CheckInRequestDto request);
        
        // Returns the paginated list of passengers for a specific flight
        Task<IEnumerable<PassengerResponseDto>> GetPassengerListAsync(string flightNumber, DateTime date, int pageNumber, int pageSize);
    }
}