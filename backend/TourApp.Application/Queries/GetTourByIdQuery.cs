using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;

namespace TourApp.Application.Queries
{
    public class GetTourByIdQuery : IRequest<TourDto>
    {
        public Guid TourId { get; set; }
    }
}
