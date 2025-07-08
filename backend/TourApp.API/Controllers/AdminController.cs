using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Commands;
using TourApp.Application.Queries;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("users/malicious")]
        public async Task<IActionResult> GetMaliciousUsers()
        {
            var query = new GetMaliciousUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }

        [HttpPut("users/{id}/block")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var command = new BlockUserCommand { UserId = id };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPut("users/{id}/unblock")]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            var command = new UnblockUserCommand { UserId = id };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}
