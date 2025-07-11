using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;
using TourApp.Domain.ValueObjects;

namespace TourApp.Domain.Entities
{
    public class Tour
    {
        private readonly List<KeyPoint> _keyPoints = new List<KeyPoint>();
        private readonly List<Rating> _ratings = new List<Rating>();

        public Guid Id { get; private set; }
        public Guid GuideId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TourDifficulty Difficulty { get; private set; }
        public Interest Category { get; private set; }
        public Money Price { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public TourStatus Status { get; private set; }
        public IReadOnlyCollection<KeyPoint> KeyPoints => _keyPoints.AsReadOnly();
        public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();
        public DateTime CreatedAt { get; private set; }

        protected Tour() { }

        public Tour(Guid guideId, string name, string description,
                   TourDifficulty difficulty, Interest category,
                   Money price, DateTime scheduledDate)
        {
            Id = Guid.NewGuid();
            GuideId = guideId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Difficulty = difficulty;
            Category = category;
            Price = price ?? throw new ArgumentNullException(nameof(price));
            ScheduledDate = scheduledDate;
            Status = TourStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddKeyPoint(KeyPoint keyPoint)
        {
            if (keyPoint == null)
                throw new ArgumentNullException(nameof(keyPoint));

            _keyPoints.Add(keyPoint);
        }

        public void Publish()
        {
            if (_keyPoints.Count < 2)
                throw new InvalidOperationException("Tour must have at least 2 key points to be published");

            Status = TourStatus.Published;
        }

        public bool CanBeCancelled()
        {
            return DateTime.UtcNow < ScheduledDate.AddHours(-24);
        }

        public void Cancel()
        {
            if (!CanBeCancelled())
                throw new InvalidOperationException("Tour can only be cancelled at least 24 hours before scheduled date");

            Status = TourStatus.Cancelled;
        }

        public double GetAverageRating()
        {
            if (!_ratings.Any())
                return 0;

            return _ratings.Average(r => r.Score);
        }

        public void AddRating(Rating rating)
        {
            if (rating == null)
                throw new ArgumentNullException(nameof(rating));

            _ratings.Add(rating);
        }
    }
}
