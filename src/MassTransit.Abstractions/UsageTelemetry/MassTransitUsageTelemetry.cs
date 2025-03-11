namespace MassTransit.UsageTelemetry;

using System;
using System.Collections.Generic;


public class MassTransitUsageTelemetry
{
    public Guid? Id { get; set; }
    public string? CustomerId { get; set; }
    public string? Created { get; set; }
    public HostUsageTelemetry? Host { get; set; }
    public List<BusUsageTelemetry>? Bus { get; set; }
    public List<RiderUsageTelemetry>? Rider { get; set; }
}
