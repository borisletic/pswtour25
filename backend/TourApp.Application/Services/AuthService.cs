using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using TourApp.Domain.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TourApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password"
                };
            }

            if (user.IsBlocked)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Your account has been blocked"
                };
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password"
                };
            }

            var userType = user.GetType().Name;
            var token = GenerateJwtToken(user.Id.ToString(), user.Username, userType);
            var expiresIn = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]);

            return new LoginResult
            {
                Success = true,
                Token = token,
                UserId = user.Id.ToString(),
                UserType = userType,
                ExpiresIn = expiresIn * 60 // Convert to seconds
            };
        }

        public string GenerateJwtToken(string userId, string username, string userType)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]);

            var claims = new[]
            {
                new Claim("UserId", userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, userType),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
