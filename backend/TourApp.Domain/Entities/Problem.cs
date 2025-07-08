using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;
using TourApp.Domain.Events;

namespace TourApp.Domain.Entities
{
    public class Problem
    {
        private readonly List<ProblemEvent> _events = new List<ProblemEvent>();

        public Guid Id { get; private set; }
        public Guid TourId { get; private set; }
        public Guid TouristId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ProblemStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<ProblemEvent> Events => _events.AsReadOnly();

        protected Problem() { }

        public Problem(Guid tourId, Guid touristId, string title, string description)
        {
            Id = Guid.NewGuid();
            TourId = tourId;
            TouristId = touristId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Status = ProblemStatus.Pending;
            CreatedAt = DateTime.UtcNow;

            AddEvent(new ProblemCreatedEvent(Id, Status));
        }

        public void MarkAsResolved()
        {
            if (Status != ProblemStatus.Pending)
                throw new InvalidOperationException("Only pending problems can be resolved");

            Status = ProblemStatus.Resolved;
            AddEvent(new ProblemStatusChangedEvent(Id, ProblemStatus.Pending, Status));
        }

        public void SendForReview()
        {
            if (Status != ProblemStatus.Pending)
                throw new InvalidOperationException("Only pending problems can be sent for review");

            Status = ProblemStatus.UnderReview;
            AddEvent(new ProblemStatusChangedEvent(Id, ProblemStatus.Pending, Status));
        }

        public void ReturnToPending()
        {
            if (Status != ProblemStatus.UnderReview)
                throw new InvalidOperationException("Only problems under review can be returned to pending");

            Status = ProblemStatus.Pending;
            AddEvent(new ProblemStatusChangedEvent(Id, ProblemStatus.UnderReview, Status));
        }

        public void Reject()
        {
            if (Status != ProblemStatus.UnderReview)
                throw new InvalidOperationException("Only problems under review can be rejected");

            Status = ProblemStatus.Rejected;
            AddEvent(new ProblemStatusChangedEvent(Id, ProblemStatus.UnderReview, Status));
        }

        private void AddEvent(ProblemEvent @event)
        {
            _events.Add(@event);
        }
    }
}
