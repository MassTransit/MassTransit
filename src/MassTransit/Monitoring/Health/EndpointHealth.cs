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
        IEndpointConfigurationObserver
    {
        readonly ConcurrentDictionary<Uri, IEndpointHealth> _endpoints;

        public EndpointHealth()
        {
            _endpoints = new ConcurrentDictionary<Uri, IEndpointHealth>();
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            configurator.ConnectReceiveEndpointObserver(this);
            _endpoints.TryAdd(configurator.InputAddress, UnHealthEndpointHealth.Instance);
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            UpdateEndpoint(ready);

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            UpdateEndpoint(stopping);

            if (stopping.Removed)
                _endpoints.TryRemove(stopping.InputAddress, out var endpoint);

            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            UpdateEndpoint(completed);

            return TaskUtil.Completed;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            UpdateEndpoint(faulted);

            return TaskUtil.Completed;
        }

        public (BusHealthStatus, string, IReadOnlyDictionary<string, EndpointHealthResult>) CheckHealth()
        {
            var results = _endpoints.Select(x => new
            {
                InputAddress = x.Key,
                Result = x.Value.CheckHealth()
            }).ToArray();

            var unhealthy = results.Where(x => x.Result.Status == BusHealthStatus.Unhealthy).ToArray();
            var degraded = results.Where(x => x.Result.Status == BusHealthStatus.Degraded).ToArray();

            var unhappy = unhealthy.Union(degraded).ToArray();

            var names = unhappy.Select(x => x.InputAddress.GetLastPart()).ToArray();

            Dictionary<string, EndpointHealthResult> data = results.ToDictionary(x => x.InputAddress.ToString(), x => x.Result);

            if (unhealthy.Any() || unhappy.Length == results.Length)
                return (BusHealthStatus.Unhealthy, $"Unhealthy Endpoints: {string.Join(",", names)}", data);

            if (degraded.Any())
                return (BusHealthStatus.Degraded, $"Degraded Endpoints: {string.Join(",", names)}", data);

            return (BusHealthStatus.Healthy, "Endpoints are healthy", data);
        }

        void UpdateEndpoint(ReceiveEndpointEvent endpointEvent)
        {
            _endpoints.AddOrUpdate(endpointEvent.InputAddress, endpointEvent.ReceiveEndpoint, (address, _) => endpointEvent.ReceiveEndpoint);
        }


        class UnHealthEndpointHealth :
            IEndpointHealth
        {
            readonly EndpointHealthResult _result;

            UnHealthEndpointHealth()
            {
                _result = EndpointHealthResult.Unhealthy(null, "not ready", null);
            }

            public static IEndpointHealth Instance { get; } = new UnHealthEndpointHealth();

            public EndpointHealthResult CheckHealth()
            {
                return _result;
            }
        }
    }
}
