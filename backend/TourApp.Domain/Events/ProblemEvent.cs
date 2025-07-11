using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TourApp.Domain.Events
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(ProblemCreatedEvent), typeDiscriminator: "created")]
    [JsonDerivedType(typeof(ProblemStatusChangedEvent), typeDiscriminator: "statusChanged")]
    public abstract class ProblemEvent
    {
        public Guid ProblemId { get; set; }
        public DateTime OccurredAt { get; set; }

        protected ProblemEvent()
        {
        }

        protected ProblemEvent(Guid problemId)
        {
            ProblemId = problemId;
            OccurredAt = DateTime.UtcNow;
        }
    }
}
