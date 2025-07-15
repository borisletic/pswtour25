using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ITouristRepository _touristRepository;
        private readonly IGuideRepository _guideRepository;

        public UserController(ITouristRepository touristRepository, IGuideRepository guideRepository)
        {
            _touristRepository = touristRepository;
            _guideRepository = guideRepository;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Fix the claim reading - use the correct claim names from JWT token
            var userIdClaim = User.FindFirst("UserId")?.Value; // Changed from "userId" to "UserId"
            var userTypeClaim = User.FindFirst(ClaimTypes.Role)?.Value; // Changed from "userType" to ClaimTypes.Role

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userTypeClaim))
            {
                return BadRequest("Invalid token claims");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid user ID format");
            }

            if (userTypeClaim == "Tourist")
            {
                var tourist = await _touristRepository.GetByIdAsync(userId);
                if (tourist == null) return NotFound("Tourist not found");

                return Ok(new
                {
                    Id = tourist.Id,
                    Username = tourist.Username,
                    Email = tourist.Email,
                    FirstName = tourist.FirstName,
                    LastName = tourist.LastName,
                    Type = "Tourist",
                    BonusPoints = tourist.BonusPoints,
                    Interests = tourist.Interests.Select(i => i.ToString()).ToList()
                });
            }
            else if (userTypeClaim == "Guide")
            {
                var guide = await _guideRepository.GetByIdAsync(userId);
                if (guide == null) return NotFound("Guide not found");

                return Ok(new
                {
                    Id = guide.Id,
                    Username = guide.Username,
                    Email = guide.Email,
                    FirstName = guide.FirstName,
                    LastName = guide.LastName,
                    Type = "Guide",
                    RewardPoints = guide.RewardPoints,
                    IsRewarded = guide.IsRewarded
                });
            }

            return BadRequest($"Invalid user type: {userTypeClaim}");
        }
    }
}
