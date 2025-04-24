namespace MassTransit.UsageTelemetry;

public class HostUsageTelemetry
{
    public string? FrameworkVersion { get; set; }
    public string? MassTransitVersion { get; set; }
    public string? OperatingSystemVersion { get; set; }
    public string? TimeZoneInfo { get; set; }
    public string? CommitHash { get; set; }
    public string? Cloud { get; set; }
    public string? Region { get; set; }
    public string? Container { get; set; }
}
