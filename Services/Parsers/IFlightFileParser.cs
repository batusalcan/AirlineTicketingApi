using AirlineTicketingApi.DTOs;

namespace AirlineTicketingApi.Services.Parsers
{
    public interface IFlightFileParser
    {
        IEnumerable<AddFlightRequestDto> Parse(Stream fileStream);
    }
}