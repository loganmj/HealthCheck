using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace HealthCheck.Server;

/// <summary>
/// Implements a custom healthcheck that attempts to ping the host.
/// </summary>
public class ICMPHealthCheck : IHealthCheck
{
    #region Fields

    /// <summary>
    /// The target host address.
    /// </summary>
    private readonly string _host = $"10.0.0.0";

    /// <summary>
    /// The maximum healthy ping time (ms).
    /// </summary>
    private readonly int _healthyRoundTripTimeMS = 300;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="healthyRoundTripTimeMS"></param>
    public ICMPHealthCheck(string host, int healthyRoundTripTimeMS)
    {
        _host = host;
        _healthyRoundTripTimeMS = healthyRoundTripTimeMS;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ping the host
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(_host);

            // Interpret the ping reply
            // If the ping is successful, return Healthy.
            // If the ping is successful, but takes to long, return Degraded.
            // If the ping is unsuccessful, return Unhealthy.
            switch (reply.Status)
            {
                case IPStatus.Success:
                    var message = $"ICMP to {_host} took {reply.RoundtripTime} ms.";
                    return (reply.RoundtripTime > _healthyRoundTripTimeMS) ? HealthCheckResult.Degraded(message) : HealthCheckResult.Healthy(message);
                default:
                    var error = $"ICMP to {_host} failed: {reply.Status}";
                    return HealthCheckResult.Unhealthy(error);
            }
        }
        catch (Exception ex)
        {
            var error = $"ICMP failed: {ex}";
            return HealthCheckResult.Unhealthy(error);
        }
    }

    #endregion
}
