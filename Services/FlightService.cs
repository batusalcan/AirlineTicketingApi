using AirlineTicketingApi.Data;
using AirlineTicketingApi.DTOs;
using AirlineTicketingApi.Models;
using AirlineTicketingApi.Services.Parsers;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingApi.Services
{
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlightFileParser _fileParser;

        // Dependency Injection brings the database context into this service
        public FlightService(ApplicationDbContext context , IFlightFileParser fileParser)
        {
            _context = context;
            _fileParser = fileParser;
        }

        public async Task<StatusResponseDto> AddFlightAsync(AddFlightRequestDto request)
        {
            try
            {
                var flight = new Flight
                {
                    FlightNumber = request.FlightNumber,
                    DateFrom = request.DateFrom,
                    DateTo = request.DateTo,
                    AirportFrom = request.AirportFrom,
                    AirportTo = request.AirportTo,
                    Duration = request.Duration,
                    Capacity = request.Capacity
                };

                _context.Flights.Add(flight);
                await _context.SaveChangesAsync();

                return new StatusResponseDto { IsSuccess = true, Message = "Flight added successfully." };
            }
            catch (Exception ex)
            {
                return new StatusResponseDto { IsSuccess = false, Message = $"Error adding flight: {ex.Message}" };
            }
        }

        public async Task<IEnumerable<FlightResponseDto>> QueryFlightsAsync(QueryFlightRequestDto request, int pageNumber, int pageSize)
        {
            // The logic: Filter by airports, dates, and crucially: Capacity MUST be >= NumberOfPeople
            var query = _context.Flights
                .Where(f => f.AirportFrom == request.AirportFrom &&
                            f.AirportTo == request.AirportTo &&
                            f.DateFrom.Date == request.DateFrom.Date &&
                            f.Capacity >= request.NumberOfPeople);

            // Implement Paging (size of 10)
            var flights = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FlightResponseDto
                {
                    FlightNumber = f.FlightNumber,
                    Duration = f.Duration
                })
                .ToListAsync();

            return flights;
        }

        public async Task<StatusResponseDto> AddFlightsFromFileAsync(Stream fileStream)
    {
        try
        {
            // Strategy Pattern handle the parsing
            var flightDtos = _fileParser.Parse(fileStream);

            // Convert the DTOs into actual database Models
            var flights = flightDtos.Select(dto => new Flight
            {
                FlightNumber = dto.FlightNumber,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                AirportFrom = dto.AirportFrom,
                AirportTo = dto.AirportTo,
                Duration = dto.Duration,
                Capacity = dto.Capacity
            }).ToList();

            // Save all of them to the database in one big chunk
            _context.Flights.AddRange(flights);
            await _context.SaveChangesAsync();

            // return "File processes status"
            return new StatusResponseDto { IsSuccess = true, Message = "File processes status: Success. All flights added." };
        }
        catch (Exception ex)
        {
            return new StatusResponseDto { IsSuccess = false, Message = $"File processes status: Error. {ex.Message}" };
        }
    }
    }
}