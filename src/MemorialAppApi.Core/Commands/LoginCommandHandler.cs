using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MemorialAppApi.Core.DTOs;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace MemorialAppApi.Core.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ILogger<LoginCommandHandler> logger,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found with email {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: User account is inactive for email {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Account is inactive"
            };
        }

        // Verify password (decode Base64 and compare)
        var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
        var passwordHash = Convert.ToBase64String(passwordBytes);

        if (passwordHash != user.PasswordHash)
        {
            _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        _logger.LogInformation("Login successful for user ID: {UserId}", user.Id);
        //generate JWT token using user info
        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful",
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PublicName = user.PublicName
            },
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Get JWT settings from configuration
            var secret = _configuration["JwtSettings:Secret"] ?? "your-256-bit-secret-key-change-this-in-production-must-be-at-least-32-characters-long";
            var issuer = _configuration["JwtSettings:Issuer"] ?? "MemorialAppApi";
            var audience = _configuration["JwtSettings:Audience"] ?? "MemorialAppApi";
            var expirationDays = int.Parse(_configuration["JwtSettings:ExpirationInDays"] ?? "7");
            
            // Ensure key is at least 128 bits (16 bytes) for HMAC-SHA256
            var key = Encoding.UTF8.GetBytes(secret);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
            throw;
        }
    }
}
