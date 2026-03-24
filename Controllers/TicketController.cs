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

        /// <summary>
        /// Buys a ticket for a specified flight and passenger.
        /// </summary>
        /// <remarks>
        /// **Midterm Requirements:**
        /// - Capacity of the flight is decreased transactionally.
        /// - Returns "Sold out" if there are no seats left.
        /// - Requires Authentication.
        /// </remarks>
        /// <param name="request">Flight number, date, and passenger name.</param>
        /// <returns>Transaction status and the generated ticket number.</returns>
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

        /// <summary>
        /// Checks in a passenger for a specific flight.
        /// </summary>
        /// <remarks>
        /// **Midterm Requirement:** Assigns a seat to the passenger using simple numbering (sequential).
        /// Does NOT require authentication.
        /// </remarks>
        /// <param name="request">Flight number, date, and passenger name.</param>
        /// <returns>Transaction status and the assigned seat number.</returns>
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
        {
            var result = await _ticketService.CheckInAsync(request);
            
            if (!result.IsSuccess) 
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the list of passengers for a specific flight.
        /// </summary>
        /// <remarks>
        /// **Midterm Requirements:**
        /// - Requires Authentication.
        /// - Supports Paging (size of 10).
        /// </remarks>
        /// <param name="flightNumber">The flight number.</param>
        /// <param name="date">The date of the flight.</param>
        /// <param name="pageNumber">Page index (default is 1).</param>
        /// <param name="pageSize">Page size (default is 10).</param>
        /// <returns>List of passengers and their assigned seats.</returns>
        [Authorize]
        [HttpGet("passengers")]
        public async Task<IActionResult> QueryPassengerList([FromQuery] string flightNumber, [FromQuery] DateTime date, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _ticketService.GetPassengerListAsync(flightNumber, date, pageNumber, pageSize);
            return Ok(result);
        }
    }
}