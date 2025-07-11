using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Commands;
using TourApp.Application.Queries;
using TourApp.Domain.Entities;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TourController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TourController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTours([FromQuery] GetToursQuery query)
        {
            Console.WriteLine($"GetTours called with:");
            Console.WriteLine($"  Category: {query.Category}");
            Console.WriteLine($"  Difficulty: {query.Difficulty}");
            Console.WriteLine($"  Status: {query.Status}");
            Console.WriteLine($"  GuideId: {query.GuideId}");
            Console.WriteLine($"  RewardedGuidesOnly: {query.RewardedGuidesOnly}");

            try
            {
                var tours = await _mediator.Send(query);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTours: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTour(Guid id)
        {
            var query = new GetTourByIdQuery { TourId = id };
            var tour = await _mediator.Send(query);

            if (tour == null)
            {
                return NotFound();
            }

            return Ok(tour);
        }

        [HttpPost]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> CreateTour([FromBody] CreateTourCommand command)
        {
            command.GuideId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetTour), new { id = result.TourId }, new { tourId = result.TourId });
        }

        [HttpPost("{tourId}/keypoints")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> AddKeyPoint(Guid tourId, [FromBody] AddKeyPointCommand command)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"AddKeyPoint called with tourId: {tourId}");
                Console.WriteLine($"Command data: Name={command?.Name}, Description={command?.Description}, Lat={command?.Latitude}, Lng={command?.Longitude}, Order={command?.Order}");

                // Validate model state
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid:");
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"  {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    return BadRequest(ModelState);
                }

                command.TourId = tourId;

                Console.WriteLine("Sending command to MediatR...");
                var result = await _mediator.Send(command);

                Console.WriteLine($"MediatR result: Success={result.Success}, Errors={string.Join(", ", result.Errors)}");

                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { keyPointId = result.KeyPointId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddKeyPoint: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id}/publish")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> PublishTour(Guid id)
        {
            var command = new PublishTourCommand { TourId = id, GuideId = GetCurrentUserId() };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpDelete("{id}/cancel")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> CancelTour(Guid id)
        {
            var command = new CancelTourCommand { TourId = id, GuideId = GetCurrentUserId() };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id}/rate")]
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> RateTour(Guid id, [FromBody] RateTourCommand command)
        {
            command.TourId = id;
            command.TouristId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.Parse(userIdClaim);
        }


    }
}
