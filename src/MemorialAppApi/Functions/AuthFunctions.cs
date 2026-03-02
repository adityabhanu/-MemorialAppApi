using MediatR;
using MemorialAppApi.Core.Commands;
using MemorialAppApi.Core.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MemorialAppApi.Functions;

public class AuthFunctions
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthFunctions> _logger;

    public AuthFunctions(IMediator mediator, ILogger<AuthFunctions> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("Register")]
    public async Task<HttpResponseData> Register(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/register")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("Register function triggered");

        try
        {
            var registerDto = await req.ReadFromJsonAsync<RegisterDto>();
            if (registerDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            var command = new RegisterCommand
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PublicName = registerDto.PublicName,
                ReceiveEmail = registerDto.ReceiveEmail,
                PhotoVolunteer = registerDto.PhotoVolunteer,
                TermsAndCondition = registerDto.TermsAndCondition
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                var conflictResponse = req.CreateResponse(HttpStatusCode.Conflict);
                await conflictResponse.WriteAsJsonAsync(result);
                return conflictResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (FluentValidation.ValidationException vex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", vex.Errors.Select(e => e.ErrorMessage)));
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Validation failed",
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new
            {
                success = false,
                message = "An error occurred during registration"
            });
            return response;
        }
    }

    [Function("Login")]
    public async Task<HttpResponseData> Login(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("Login function triggered");

        try
        {
            var loginDto = await req.ReadFromJsonAsync<LoginDto>();
            if (loginDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            var command = new LoginCommand
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteAsJsonAsync(result);
                return unauthorizedResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (FluentValidation.ValidationException vex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", vex.Errors.Select(e => e.ErrorMessage)));
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Validation failed",
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new
            {
                success = false,
                message = "An error occurred during login"
            });
            return response;
        }
    }

    [Function("UpdateUser")]
    public async Task<HttpResponseData> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "auth/users/{userId}")] HttpRequestData req,
        FunctionContext executionContext,
        string userId)
    {
        _logger.LogInformation("UpdateUser function triggered for UserId: {UserId}", userId);

        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid user ID format" });
                return badResponse;
            }

            var updateDto = await req.ReadFromJsonAsync<UserDto>();
            if (updateDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            var command = new UpdateUserCommand
            {
                UserId = userGuid,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                PublicName = updateDto.PublicName,
                ProfilePic = updateDto.ProfilePic,
                ReceiveEmail = updateDto.ReceiveEmail,
                PhotoVolunteer = updateDto.PhotoVolunteer,
                TermsAndCondition = updateDto.TermsAndCondition
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (FluentValidation.ValidationException vex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", vex.Errors.Select(e => e.ErrorMessage)));
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new
            {
                error = "Validation failed",
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
            return response;
        }
        catch (MemorialAppApi.Core.Exceptions.NotFoundException nfex)
        {
            _logger.LogWarning("User not found: {Message}", nfex.Message);
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = nfex.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new
            {
                error = "An error occurred while updating user profile"
            });
            return response;
        }
    }
}
