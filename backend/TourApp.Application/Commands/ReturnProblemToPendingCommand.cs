using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class ReturnProblemToPendingCommand : IRequest<CommandResult>
    {
        public Guid ProblemId { get; set; }
    }
}
