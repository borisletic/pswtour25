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
    [Authorize(Roles = "Tourist")]
    public class PurchaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseTours([FromBody] PurchaseTourCommand command)
        {
            command.TouristId = GetCurrentUserId();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new
            {
                purchaseId = result.PurchaseId,
                totalPaid = result.TotalPaid,
                bonusPointsUsed = result.BonusPointsUsed
            });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPurchaseHistory()
        {
            var query = new GetPurchaseHistoryQuery { TouristId = GetCurrentUserId() };
            var purchases = await _mediator.Send(query);
            return Ok(purchases);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.Parse(userIdClaim);
        }
    }
}
