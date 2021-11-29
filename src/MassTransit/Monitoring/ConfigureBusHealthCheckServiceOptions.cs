namespace MassTransit.Monitoring
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Transports;


    public class ConfigureBusHealthCheckServiceOptions :
        IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<IBusInstance> _busInstances;
        readonly string[] _tags;

        public ConfigureBusHealthCheckServiceOptions(IEnumerable<IBusInstance> busInstances)
        {
            _busInstances = busInstances;
            _tags = new[] { "ready", "masstransit" };
        }

        public void Configure(HealthCheckServiceOptions options)
        {
            foreach (var busInstance in _busInstances)
                options.Registrations.Add(new HealthCheckRegistration(busInstance.Name, new BusHealthCheck(busInstance), HealthStatus.Unhealthy, _tags));
        }
    }
}
