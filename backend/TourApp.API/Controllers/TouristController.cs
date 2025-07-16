using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Commands;
using TourApp.Application.Queries;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Tourist")]
    public class TouristController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TouristController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("interests")]
        public async Task<IActionResult> UpdateInterests([FromBody] UpdateInterestsCommand command)
        {
            command.TouristId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var query = new GetTouristProfileQuery { TouristId = GetCurrentUserId() };
            var profile = await _mediator.Send(query);
            return Ok(profile);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.Parse(userIdClaim);
        }
    }
}