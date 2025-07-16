using TourApp.Domain.Enums;

namespace TourApp.Application.DTOs
{
    public class TouristProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Interest> Interests { get; set; } = new List<Interest>();
        public decimal BonusPoints { get; set; }
    }
}