using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Events
{
    public class ProblemCreatedEvent : ProblemEvent
    {
        public ProblemStatus InitialStatus { get; }

        public ProblemCreatedEvent(Guid problemId, ProblemStatus initialStatus)
            : base(problemId)
        {
            InitialStatus = initialStatus;
        }
    }
}
