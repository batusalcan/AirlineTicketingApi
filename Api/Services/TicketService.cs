using AirlineTicketingApi.Data;
using AirlineTicketingApi.DTOs;
using AirlineTicketingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BuyTicketResponseDto> BuyTicketAsync(BuyTicketRequestDto request)
        {
            // 1. Find the flight matching the date and number
            var flight = await _context.Flights
                .FirstOrDefaultAsync(f => f.FlightNumber == request.FlightNumber && f.DateFrom.Date == request.Date.Date);

            if (flight == null)
                return new BuyTicketResponseDto { TransactionStatus = "Error: Flight not found." };

            // 2. MIDTERM REQUIREMENT: Return "Sold out" if there are no seats left
            if (flight.Capacity <= 0)
                return new BuyTicketResponseDto { TransactionStatus = "Sold out" };

            // 3. Find the user, or create a new one if they don't exist yet
            var passenger = await _context.Users.FirstOrDefaultAsync(u => u.FullName == request.PassengerName);
            if (passenger == null)
            {
                passenger = new User { FullName = request.PassengerName, Role = "Passenger" };
                _context.Users.Add(passenger);
            }

            // 4. Create the Ticket and generate a random 6-character ticket number
            var ticket = new Ticket
            {
                TicketNumber = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                Flight = flight,
                Passenger = passenger
            };

            // 5. MIDTERM REQUIREMENT: Decrease flight capacity
            flight.Capacity -= 1;
            
            _context.Tickets.Add(ticket);
            
            try
            {
                // This saves both the new user, the new ticket, and the updated flight capacity at the exact same time
                await _context.SaveChangesAsync(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                // If two people try to take the last ticket at the same millisecond, one of them will get this mistake
                return new BuyTicketResponseDto 
                { 
                    TransactionStatus = "Error: High traffic concurrent request. The seat was modified by another user just now. Please try again." 
                };
            }

            return new BuyTicketResponseDto 
            { 
                TransactionStatus = "Success", 
                TicketNumber = ticket.TicketNumber 
            };
        }

        public async Task<StatusResponseDto> CheckInAsync(CheckInRequestDto request)
        {
            // 1. Find the specific ticket using the Flight and Passenger info
            var ticket = await _context.Tickets
                .Include(t => t.Flight)
                .Include(t => t.Passenger)
                .FirstOrDefaultAsync(t => t.Flight!.FlightNumber == request.FlightNumber &&
                                          t.Flight.DateFrom.Date == request.Date.Date &&
                                          t.Passenger!.FullName == request.PassengerName);

            if (ticket == null)
                return new StatusResponseDto { IsSuccess = false, Message = "Ticket not found." };

            if (ticket.SeatNumber != null)
                return new StatusResponseDto { IsSuccess = false, Message = "Passenger is already checked in." };

            // 2. MIDTERM REQUIREMENT: Assign seat (simple numbering)
            // We find the highest seat number currently assigned on this flight, and add 1
            var maxSeat = await _context.Tickets
                .Where(t => t.FlightId == ticket.FlightId && t.SeatNumber != null)
                .MaxAsync(t => (int?)t.SeatNumber) ?? 0;

            ticket.SeatNumber = maxSeat + 1;
            
            await _context.SaveChangesAsync();

            return new StatusResponseDto 
            { 
                IsSuccess = true, 
                Message = $"Checked in successfully. Assigned Seat: {ticket.SeatNumber}" 
            };
        }

        public async Task<IEnumerable<PassengerResponseDto>> GetPassengerListAsync(string flightNumber, DateTime date, int pageNumber, int pageSize)
        {
            var query = _context.Tickets
                .Include(t => t.Passenger)
                .Include(t => t.Flight)
                .Where(t => t.Flight!.FlightNumber == flightNumber && t.Flight.DateFrom.Date == date.Date);

            // MIDTERM REQUIREMENT: Paging (size of 10)
            var passengers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new PassengerResponseDto
                {
                    PassengerName = t.Passenger!.FullName,
                    SeatNumber = t.SeatNumber
                })
                .ToListAsync();

            return passengers;
        }
    }
}