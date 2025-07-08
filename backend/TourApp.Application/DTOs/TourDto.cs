using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.DTOs
{
    public class TourDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; }
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; }
        public GuideInfoDto Guide { get; set; }
    }

    public class KeyPointDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
    }

    public class GuideInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRewarded { get; set; }
    }
}
