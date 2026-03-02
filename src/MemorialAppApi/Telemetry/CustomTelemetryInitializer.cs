using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace MemorialAppApi.Telemetry;

public class CustomTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = "MemorialAppApi";
        telemetry.Context.Cloud.RoleInstance = Environment.MachineName;
        
        // Add custom properties
        telemetry.Context.GlobalProperties["Application"] = "MemorialAppApi";
        telemetry.Context.GlobalProperties["Environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    }
}
