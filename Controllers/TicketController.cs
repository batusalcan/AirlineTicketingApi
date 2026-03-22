using AirlineTicketingApi.DTOs;
using AirlineTicketingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AirlineTicketingApi.Controllers
{
    [ApiController]
    [Route("api/v1/ticket")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // POST: api/v1/ticket/buy
        [Authorize] 
        [HttpPost("buy")]
        public async Task<IActionResult> BuyTicket([FromBody] BuyTicketRequestDto request)
        {
            var result = await _ticketService.BuyTicketAsync(request);
            
            //Check if it says "Sold out" or has an error
            if (result.TransactionStatus != "Success") 
                return BadRequest(result); 

            return Ok(result);
        }

        // POST: api/v1/ticket/checkin
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
        {
            var result = await _ticketService.CheckInAsync(request);
            
            if (!result.IsSuccess) 
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/v1/ticket/passengers
        [Authorize]
        [HttpGet("passengers")]
        public async Task<IActionResult> QueryPassengerList([FromQuery] string flightNumber, [FromQuery] DateTime date, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _ticketService.GetPassengerListAsync(flightNumber, date, pageNumber, pageSize);
            return Ok(result);
        }
    }
}