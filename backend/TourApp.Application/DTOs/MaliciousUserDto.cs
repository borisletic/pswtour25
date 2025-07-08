using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.DTOs
{
    public class MaliciousUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public int Count { get; set; }
        public bool IsBlocked { get; set; }
    }
}
