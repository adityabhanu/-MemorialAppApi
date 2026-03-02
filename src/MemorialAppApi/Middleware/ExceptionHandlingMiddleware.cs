using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace MemorialAppApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly TelemetryClient _telemetryClient;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        TelemetryClient telemetryClient)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["ErrorType"] = ex.GetType().Name,
                ["Source"] = ex.Source ?? "Unknown"
            });

            throw;
        }
    }
}
