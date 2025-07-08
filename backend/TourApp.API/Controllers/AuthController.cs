using MediatR;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Commands;
using TourApp.Application.Services;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public AuthController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterTouristCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { touristId = result.TouristId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

            if (!result.Success)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new
            {
                token = result.Token,
                userId = result.UserId,
                userType = result.UserType,
                expiresIn = result.ExpiresIn
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
