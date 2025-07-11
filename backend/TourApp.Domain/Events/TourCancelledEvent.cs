using System;

namespace TourApp.Domain.Events
{
    public class TourCancelledEvent
    {
        public Guid TourId { get; protected set; }
        public DateTime CancelledAt { get; protected set; }
        public string Reason { get; protected set; }

        // Parameterless constructor for JSON deserialization
        public TourCancelledEvent() { }

        public TourCancelledEvent(Guid tourId, string reason = null)
        {
            TourId = tourId;
            CancelledAt = DateTime.UtcNow;
            Reason = reason;
        }
    }
}