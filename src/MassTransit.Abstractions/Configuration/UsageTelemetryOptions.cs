namespace MassTransit;

using System;


public class UsageTelemetryOptions
{
    /// <summary>
    /// Defaults to true, if set to false, all usage telemetry is disabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Specify your CustomerId if you're an existing customer. If not specified, may be pulled from the license key if present.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// The delay after the bus has started before reporting usage telemetry
    /// </summary>
    public TimeSpan? ReportDelay { get; set; }

    /// <summary>
    /// If true, usage telemetry will be reported when the application is stopped (if it hasn't already been reported)
    /// </summary>
    public bool ReportOnShutdown { get; set; }

    /// <summary>
    /// If true, consumer, saga, and activity details are included in the usage telemetry. Defaults to false.
    /// </summary>
    public bool Verbose { get; set; }
}
