using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Exceptions;

namespace MemorialAppApi.Core.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user profile for UserId: {UserId}", request.UserId);

        // Get user by ID
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            throw new NotFoundException($"User with ID {request.UserId} not found");
        }

        // Update fields if provided (excluding Id, Email, PasswordHash, CreatedAt, IsActive)
        if (!string.IsNullOrEmpty(request.FirstName))
        {
            user.FirstName = request.FirstName;
        }

        if (!string.IsNullOrEmpty(request.LastName))
        {
            user.LastName = request.LastName;
        }

        if (!string.IsNullOrEmpty(request.PublicName))
        {
            user.PublicName = request.PublicName;
        }

        if (!string.IsNullOrEmpty(request.ProfilePic))
        {
            user.ProfilePic = request.ProfilePic;
        }

        if (request.ReceiveEmail.HasValue)
        {
            user.ReceiveEmail = request.ReceiveEmail.Value;
        }

        if (request.PhotoVolunteer.HasValue)
        {
            user.PhotoVolunteer = request.PhotoVolunteer.Value;
        }

        if (request.TermsAndCondition.HasValue)
        {
            user.TermsAndCondition = request.TermsAndCondition.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User profile updated successfully for UserId: {UserId}", updatedUser.Id);

        return new UserDto
        {
            Id = updatedUser.Id,
            Email = updatedUser.Email,
            FirstName = updatedUser.FirstName,
            LastName = updatedUser.LastName,
            PublicName = updatedUser.PublicName,
            ProfilePic = updatedUser.ProfilePic,
            ReceiveEmail = updatedUser.ReceiveEmail,
            PhotoVolunteer = updatedUser.PhotoVolunteer,
            TermsAndCondition = updatedUser.TermsAndCondition,
            CreatedAt = updatedUser.CreatedAt
        };
    }
}
