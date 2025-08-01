﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class RateTourCommand : IRequest<CommandResult>
    {
        public Guid TourId { get; set; }
        public Guid TouristId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
    }
}
