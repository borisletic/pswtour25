using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Events
{
    public abstract class ProblemEvent
    {
        public Guid ProblemId { get; }
        public DateTime OccurredAt { get; }

        protected ProblemEvent(Guid problemId)
        {
            ProblemId = problemId;
            OccurredAt = DateTime.UtcNow;
        }
    }
}
