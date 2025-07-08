using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;

namespace TourApp.Application.Commands
{
    public class RegisterTouristCommand : IRequest<RegisterTouristResult>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Interest> Interests { get; set; }
    }

    public class RegisterTouristResult
    {
        public bool Success { get; set; }
        public string TouristId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
