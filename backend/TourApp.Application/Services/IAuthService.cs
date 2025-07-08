using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string username, string password);
        string GenerateJwtToken(string userId, string username, string userType);
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public int ExpiresIn { get; set; }
        public string ErrorMessage { get; set; }
    }
}
