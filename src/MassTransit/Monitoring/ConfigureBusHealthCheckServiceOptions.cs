namespace MassTransit.Monitoring
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Transports;

    public class ConfigureBusHealthCheckServiceOptions :
        IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<IBusInstance> _busInstances;
        readonly IOptions<MassTransitHealthCheckOptions> _options;

        public ConfigureBusHealthCheckServiceOptions(IEnumerable<IBusInstance> busInstances, IOptions<MassTransitHealthCheckOptions> options)
        {
            _busInstances = busInstances;
            _options = options;
        }


        public void Configure(HealthCheckServiceOptions options)
        {
            string[] tags = _options.Value.Tags.ToArray() ?? new[] { "ready", "masstransit" };
            foreach (var busInstance in _busInstances)
            {
                options.Registrations.Add(new HealthCheckRegistration(
                    _options.Value.Name ?? busInstance.Name,
                    new BusHealthCheck(busInstance),
                    _options.Value.FailureStatus ?? HealthStatus.Unhealthy,
                    tags));
            }
        }
    }
}
