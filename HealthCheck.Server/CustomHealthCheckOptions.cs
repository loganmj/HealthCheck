using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;

namespace HealthCheck.Server;

/// <summary>
/// Provides custom health check options.
/// </summary>
public class CustomHealthCheckOptions : HealthCheckOptions
{
    #region Constructors

    /// <summary>
    /// Constructor, calls the HealthCheckOptions base constructor.
    /// </summary>
    public CustomHealthCheckOptions() : base()
    {
        // Create an options object for a JSON serializer.
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        // Customizes the written response to healthcheck statuses
        ResponseWriter = async (context, response) =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status200OK;

            // Serializes the result as a JSON object using the following sculpted data
            // and the previously defined JSON serializer options.
            //
            // Individual check data:
            // name: The identifier string we entered when adding the healthcheck to the builder.
            // responseTime: The duration of the check.
            // status: The status of the check.
            // description: The custom message returned by the check.
            //
            // Combined check data:
            // totalStatus: The boolean sum of all check responses.
            // totalResponseTime: The total duration of all checks.

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                checks = response.Entries.Select(entry => new
                {
                    name = entry.Key,
                    responseTime = entry.Value.Duration.TotalMilliseconds,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description
                }),
                totalStatus = response.Status,
                totalResponseTime = response.TotalDuration.TotalMilliseconds,
            }, jsonSerializerOptions));
        };
    }

    #endregion
}
