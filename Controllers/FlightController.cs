using AirlineTicketingApi.DTOs;
using AirlineTicketingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AirlineTicketingApi.Controllers
{
    [ApiController]
    [Route("api/v1/flight")] // Versioning 
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        // Dependency Injection brings in your business logic
        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// Adds a single flight to the airline schedule.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Authentication (Admin token).
        /// </remarks>
        /// <param name="request">Flight details including origin, destination, duration, and capacity.</param>
        /// <returns>Returns the transaction status.</returns>
        [Authorize] 
        [HttpPost]
        public async Task<IActionResult> AddFlight([FromBody] AddFlightRequestDto request)
        {
            var result = await _flightService.AddFlightAsync(request);
            
            if (!result.IsSuccess) 
                return BadRequest(result); // Returns a 400 error
                
            return Ok(result); // Returns a 200 success
        }

        /// <summary>
        /// Queries available flights based on origin, destination, dates, and required capacity.
        /// </summary>
        /// <remarks>
        /// **Midterm Requirements:**
        /// - Flights that have no seats (Capacity &lt; NumberOfPeople) are NOT listed.
        /// - Supports Paging with a default size of 10.
        /// - Rate Limiting: Limit calls to 3 per day (Handled via API Gateway).
        /// </remarks>
        /// <param name="request">Query parameters including dates and number of people.</param>
        /// <param name="pageNumber">Page index (default is 1).</param>
        /// <param name="pageSize">Page size (default is 10).</param>
        /// <returns>A paginated list of available flights.</returns>
        [HttpGet]
        public async Task<IActionResult> QueryFlight([FromQuery] QueryFlightRequestDto request, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // The parameters automatically set a default Paging size of 10
            var result = await _flightService.QueryFlightsAsync(request, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Adds multiple flights in batch via a .csv file upload.
        /// </summary>
        /// <remarks>
        /// Implements the Strategy Pattern for file parsing. The CSV file must contain the fields: 
        /// Flight number, date-from, date-to, airport-from, airport-to, duration, capacity.
        /// Requires Authentication.
        /// </remarks>
        /// <param name="file">The .csv file containing flight schedules.</param>
        /// <returns>File processes status.</returns>
        [Authorize] 
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFlights(IFormFile file)
        {
            // Validate that a file was actually sent
            if (file == null || file.Length == 0)
               return BadRequest(new StatusResponseDto { IsSuccess = false, Message = "No file uploaded." });

            // Open the file stream and pass it to the service
            using var stream = file.OpenReadStream();
    
            var result = await _flightService.AddFlightsFromFileAsync(stream);
    
            if (!result.IsSuccess)
              return BadRequest(result);

            return Ok(result);
        }
    }
}