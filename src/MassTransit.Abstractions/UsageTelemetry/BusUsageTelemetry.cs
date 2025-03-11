namespace MassTransit.UsageTelemetry;

using System.Collections.Generic;


public class BusUsageTelemetry
{
    public string? Created { get; set; }
    public string? Configured { get; set; }
    public string? Started { get; set; }
    public string? Stopped { get; set; }

    public string? Name { get; set; }
    public List<EndpointUsageTelemetry>? Endpoints { get; set; }
}
