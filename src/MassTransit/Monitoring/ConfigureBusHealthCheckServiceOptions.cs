namespace MassTransit.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MassTransit.Configuration;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Transports;


    public class ConfigureBusHealthCheckServiceOptions :
        IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<IBusInstance> _busInstances;
        readonly IServiceProvider _provider;
        readonly string[] _tags;

        public ConfigureBusHealthCheckServiceOptions(IEnumerable<IBusInstance> busInstances, IServiceProvider provider)
        {
            _busInstances = busInstances;
            _provider = provider;
            _tags = new[] { "ready", "masstransit" };
        }

        public void Configure(HealthCheckServiceOptions options)
        {
            foreach (var busInstance in _busInstances)
            {
                var type = typeof(MassTransitHealthCheckOptions<>).MakeGenericType(busInstance.InstanceType);
                var optionsType = typeof(IOptions<>).MakeGenericType(type);

                var name = busInstance.Name;
                HealthStatus? failureStatus = HealthStatus.Unhealthy;
                var tags = new HashSet<string>(_tags, StringComparer.OrdinalIgnoreCase);

                var busOptions = _provider.GetService(optionsType);
                if (busOptions != null)
                {
                    var healthCheckOptions = (IHealthCheckOptions)optionsType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)
                        .GetValue(busOptions, null);

                    if (!string.IsNullOrWhiteSpace(healthCheckOptions.Name))
                        name = healthCheckOptions.Name;

                    if (healthCheckOptions.FailureStatus.HasValue)
                        failureStatus = healthCheckOptions.FailureStatus.Value;

                    if (healthCheckOptions.Tags.Any())
                        tags = healthCheckOptions.Tags;
                }

                options.Registrations.Add(new HealthCheckRegistration(name, new BusHealthCheck(busInstance), failureStatus, tags));
            }
        }
    }
}
