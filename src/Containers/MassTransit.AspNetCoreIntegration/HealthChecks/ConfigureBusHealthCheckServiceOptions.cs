namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Registration;


    public class ConfigureBusHealthCheckServiceOptions :
        IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<IBusInstance> _busInstances;
        readonly string[] _tags;

        public ConfigureBusHealthCheckServiceOptions(IEnumerable<IBusInstance> busInstances, string[] tags)
        {
            _busInstances = busInstances;
            _tags = tags;
        }

        public void Configure(HealthCheckServiceOptions options)
        {
            foreach (var busInstance in _busInstances)
            {
                options.Registrations
                    .Add(new HealthCheckRegistration(busInstance.Name, new BusHealthCheck(busInstance), HealthStatus.Unhealthy, _tags));
            }
        }
    }
}
