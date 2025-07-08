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
    public class ProblemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProblemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> ReportProblem([FromBody] ReportProblemCommand command)
        {
            command.TouristId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { problemId = result.ProblemId });
        }

        [HttpGet("tourist")]
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> GetTouristProblems()
        {
            var query = new GetProblemsByTouristQuery { TouristId = GetCurrentUserId() };
            var problems = await _mediator.Send(query);
            return Ok(problems);
        }

        [HttpGet("guide")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> GetGuideProblems()
        {
            var query = new GetProblemsByGuideQuery { GuideId = GetCurrentUserId() };
            var problems = await _mediator.Send(query);
            return Ok(problems);
        }

        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> ResolveProblem(Guid id)
        {
            var command = new ResolveProblemCommand { ProblemId = id, GuideId = GetCurrentUserId() };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPut("{id}/review")]
        [Authorize(Roles = "Guide")]
        public async Task<IActionResult> SendForReview(Guid id)
        {
            var command = new SendProblemForReviewCommand { ProblemId = id, GuideId = GetCurrentUserId() };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpGet("admin/pending")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetPendingProblems()
        {
            var query = new GetProblemsUnderReviewQuery();
            var problems = await _mediator.Send(query);
            return Ok(problems);
        }

        [HttpPut("{id}/admin/return")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ReturnToPending(Guid id)
        {
            var command = new ReturnProblemToPendingCommand { ProblemId = id };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPut("{id}/admin/reject")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RejectProblem(Guid id)
        {
            var command = new RejectProblemCommand { ProblemId = id };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.Parse(userIdClaim);
        }
    }
}
