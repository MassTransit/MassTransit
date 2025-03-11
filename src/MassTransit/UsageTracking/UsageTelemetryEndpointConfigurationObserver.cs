namespace MassTransit.UsageTracking;

using System.Collections.Generic;
using UsageTelemetry;


public class UsageTelemetryEndpointConfigurationObserver :
    IEndpointConfigurationObserver,
    IUsageTelemetrySource
{
    readonly BusUsageTelemetry _busUsageTelemetry;
    readonly MassTransitUsageTelemetry _usageTelemetry;
    readonly List<IUsageTelemetrySource> _sources;

    public UsageTelemetryEndpointConfigurationObserver(BusUsageTelemetry busUsageTelemetry, MassTransitUsageTelemetry usageTelemetry)
    {
        _busUsageTelemetry = busUsageTelemetry;
        _usageTelemetry = usageTelemetry;
        _sources = [];
    }

    public void EndpointConfigured<T>(T configurator)
        where T : IReceiveEndpointConfigurator
    {
        var endpointUsageInfo = _busUsageTelemetry.AddEndpoint(configurator, _usageTelemetry);

        if (endpointUsageInfo is not null)
            _sources.Add(new UsageTelemetryConfigurationObserver(configurator, endpointUsageInfo));
    }

    public void Update()
    {
        _sources.ForEach(x => x.Update());
    }
}
