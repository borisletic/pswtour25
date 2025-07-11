using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Events
{
    public class ProblemStatusChangedEvent : ProblemEvent
    {
        public ProblemStatus OldStatus { get; set; }
        public ProblemStatus NewStatus { get; set; }

        public ProblemStatusChangedEvent()
        {
        }

        public ProblemStatusChangedEvent(Guid problemId, ProblemStatus oldStatus, ProblemStatus newStatus)
            : base(problemId)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}
