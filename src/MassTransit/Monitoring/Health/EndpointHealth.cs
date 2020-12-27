namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using Registration;
    using Util;


    public class EndpointHealth :
        IReceiveEndpointObserver,
        IEndpointConfigurationObserver,
        IEndpointHealth
    {
        readonly ConcurrentDictionary<Uri, Endpoint> _endpoints;

        public EndpointHealth()
        {
            _endpoints = new ConcurrentDictionary<Uri, Endpoint>();
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            _endpoints.GetOrAdd(configurator.InputAddress, address => new Endpoint());
        }

        public HealthResult CheckHealth()
        {
            var results = _endpoints.Select(x => new
            {
                InputAddress = x.Key,
                Result = x.Value.ReceiveEndpoint?.CheckHealth() ?? HealthResult.Unhealthy("not ready")
            }).ToArray();

            var unhealthy = results.Where(x => x.Result.Status == BusHealthStatus.Unhealthy).ToArray();
            var degraded = results.Where(x => x.Result.Status == BusHealthStatus.Degraded).ToArray();

            var unhappy = unhealthy.Union(degraded).ToArray();

            var names = unhappy.Select(x => x.InputAddress.GetLastPart()).ToArray();

            Dictionary<string, object> data = results.ToDictionary(x => x.InputAddress.ToString(), x => (object)x.Result);

            HealthResult healthCheckResult;
            if (unhealthy.Any() || unhappy.Length == results.Length)
            {
                healthCheckResult = HealthResult.Unhealthy($"Unhealthy Endpoints: {string.Join(",", names)}",
                    unhappy.Select(x => x.Result.Exception).FirstOrDefault(e => e != null), data);
            }
            else if (degraded.Any())
                healthCheckResult = HealthResult.Degraded($"Degraded Endpoints: {string.Join(",", names)}", data);
            else
                healthCheckResult = HealthResult.Healthy("Endpoints are healthy", data);

            return healthCheckResult;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            GetEndpoint(ready);

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            GetEndpoint(stopping);

            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            GetEndpoint(completed);

            return TaskUtil.Completed;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            GetEndpoint(faulted);

            return TaskUtil.Completed;
        }

        void GetEndpoint(ReceiveEndpointEvent endpointEvent)
        {
            var endpoint = _endpoints.GetOrAdd(endpointEvent.InputAddress, address => new Endpoint());

            endpoint.ReceiveEndpoint ??= endpointEvent.ReceiveEndpoint;
        }


        class Endpoint
        {
            public IReceiveEndpoint ReceiveEndpoint { get; set; }
        }
    }
}
