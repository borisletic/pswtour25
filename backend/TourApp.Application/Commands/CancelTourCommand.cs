using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class CancelTourCommand : IRequest<CommandResult>
    {
        public Guid TourId { get; set; }
        public Guid GuideId { get; set; }
    }
}
