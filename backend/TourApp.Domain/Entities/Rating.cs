using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; private set; }
        public Guid TourId { get; private set; }
        public Guid TouristId { get; private set; }
        public int Score { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Rating() { }

        public Rating(Guid tourId, Guid touristId, int score, string comment = null)
        {
            Id = Guid.NewGuid();
            TourId = tourId;
            TouristId = touristId;
            SetScore(score);
            SetComment(score, comment);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetScore(int score)
        {
            if (score < 1 || score > 5)
                throw new ArgumentException("Score must be between 1 and 5");

            Score = score;
        }

        private void SetComment(int score, string comment)
        {
            if ((score == 1 || score == 2) && string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment is required for scores 1 and 2");

            Comment = comment;
        }

        public bool CanBeEdited()
        {
            // Rating can be edited within 30 days of tour completion
            return DateTime.UtcNow <= CreatedAt.AddDays(30);
        }
    }
}
