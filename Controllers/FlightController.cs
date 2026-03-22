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

        // POST: api/v1/flight
        [Authorize] 
        [HttpPost]
        public async Task<IActionResult> AddFlight([FromBody] AddFlightRequestDto request)
        {
            var result = await _flightService.AddFlightAsync(request);
            
            if (!result.IsSuccess) 
                return BadRequest(result); // Returns a 400 error
                
            return Ok(result); // Returns a 200 success
        }

        // GET: api/v1/flight
        [HttpGet]
        public async Task<IActionResult> QueryFlight([FromQuery] QueryFlightRequestDto request, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // The parameters automatically set a default Paging size of 10
            var result = await _flightService.QueryFlightsAsync(request, pageNumber, pageSize);
            return Ok(result);
        }

        // POST: api/v1/flight/upload
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