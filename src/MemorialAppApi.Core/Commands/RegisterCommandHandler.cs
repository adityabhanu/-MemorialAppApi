using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using System.Text;

namespace MemorialAppApi.Core.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

        // Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email already registered"
            };
        }

        // Encode password to Base64
        var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
        var passwordHash = Convert.ToBase64String(passwordBytes);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PublicName = request.PublicName,
            ReceiveEmail = request.ReceiveEmail,
            TermsAndCondition = request.TermsAndCondition,
            PhotoVolunteer = request.PhotoVolunteer,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration successful",
            User = new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                PublicName = createdUser.PublicName,
                CreatedAt = createdUser.CreatedAt
            }
        };
    }
}
