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

        public bool BusHealthy { get; set; }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            configurator.ConnectReceiveEndpointObserver(this);
            _endpoints.TryAdd(configurator.InputAddress, BusHealthy ? ConnectingEndpointHealth.Instance : UnHealthEndpointHealth.Instance);
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            UpdateEndpoint(ready);

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            if (stopping.Removed && _endpoints.TryGetValue(stopping.InputAddress, out var endpoint))
                _endpoints.TryUpdate(stopping.InputAddress, new RemoveWhenCompletedEndpointHealth(stopping.ReceiveEndpoint), endpoint);

            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            if (_endpoints.TryGetValue(completed.InputAddress, out var endpoint) && endpoint is RemoveWhenCompletedEndpointHealth)
                _endpoints.TryRemove(completed.InputAddress, out var removed);

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


        class ConnectingEndpointHealth :
            IEndpointHealth
        {
            readonly EndpointHealthResult _result;

            ConnectingEndpointHealth()
            {
                _result = EndpointHealthResult.Healthy(null, "starting");
            }

            public static IEndpointHealth Instance { get; } = new ConnectingEndpointHealth();

            public EndpointHealthResult CheckHealth()
            {
                return _result;
            }
        }


        class RemoveWhenCompletedEndpointHealth :
            IEndpointHealth
        {
            readonly IReceiveEndpoint _receiveEndpoint;

            public RemoveWhenCompletedEndpointHealth(IReceiveEndpoint receiveEndpoint)
            {
                _receiveEndpoint = receiveEndpoint;
            }

            public EndpointHealthResult CheckHealth()
            {
                return _receiveEndpoint.CheckHealth();
            }
        }
    }
}
