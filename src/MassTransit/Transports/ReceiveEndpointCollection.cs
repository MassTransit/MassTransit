namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Observables;
    using Util;


    public class ReceiveEndpointCollection :
        IReceiveEndpointCollection
    {
        readonly SingleThreadedDictionary<string, ReceiveEndpoint> _endpoints;
        readonly ReceiveEndpointObservable _receiveEndpointObservers;
        bool _started;

        public ReceiveEndpointCollection()
        {
            _receiveEndpointObservers = new ReceiveEndpointObservable();

            _endpoints = new SingleThreadedDictionary<string, ReceiveEndpoint>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string endpointName, ReceiveEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            endpoint.HealthResult = _started
                ? EndpointHealthResult.Healthy(endpoint, "starting")
                : EndpointHealthResult.Unhealthy(endpoint, "not ready", null);

            var added = _endpoints.TryAdd(endpointName, _ =>
            {
                endpoint.ConnectReceiveEndpointObserver(new HealthResultReceiveEndpointObserver(endpoint));
                endpoint.ObserverHandle = endpoint.ConnectReceiveEndpointObserver(_receiveEndpointObservers);

                return endpoint;
            });

            if (!added)
                throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");
        }

        public HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
        {
            _started = true;

            KeyValuePair<string, ReceiveEndpoint>[] endpointsToStart = _endpoints.Where(x => !x.Value.IsStarted()).ToArray();

            return endpointsToStart.Select(x => StartEndpoint(x.Key, x.Value, cancellationToken)).ToArray();
        }

        public HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            if (!_endpoints.TryGetValue(endpointName, out var endpoint))
                throw new ConfigurationException($"A receive endpoint with the key was not found: {endpointName}");

            if (endpoint.IsStarted())
                throw new ArgumentException($"The specified endpoint has already been started: {endpointName}", nameof(endpointName));

            return StartEndpoint(endpointName, endpoint, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            foreach (KeyValuePair<string, ReceiveEndpoint> endpoint in _endpoints)
            {
                var endpointScope = context.CreateScope("receiveEndpoint");
                endpointScope.Add("name", endpoint.Key);
                if (endpoint.Value.IsStarted())
                    endpointScope.Add("started", true);

                endpoint.Value.Probe(endpointScope);
            }
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return new MultipleConnectHandle(_endpoints.Values.Select(x => x.ConnectConsumeMessageObserver(observer)));
        }

        public IEnumerable<EndpointHealthResult> CheckEndpointHealth()
        {
            return _endpoints.Values.Select(x => x.HealthResult).ToList();
        }

        public async Task StopEndpoints(CancellationToken cancellationToken)
        {
            ReceiveEndpoint[] endpoints = _endpoints.Values.Where(x => x.IsStarted() && !x.IsBusEndpoint).ToArray();

            await Task.WhenAll(endpoints.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

            endpoints = _endpoints.Values.Where(x => x.IsStarted() && x.IsBusEndpoint).ToArray();

            await Task.WhenAll(endpoints.Select(x => x.Stop(cancellationToken))).ConfigureAwait(false);

            _started = false;
        }

        HostReceiveEndpointHandle StartEndpoint(string endpointName, ReceiveEndpoint endpoint, CancellationToken cancellationToken)
        {
            try
            {
                void RemoveEndpoint()
                {
                    endpoint.ObserverHandle.Disconnect();
                    Remove(endpointName);
                }

                var handle = new Handle(endpoint, RemoveEndpoint);

                handle.Start(cancellationToken);

                return handle;
            }
            catch
            {
                _endpoints.TryRemove(endpointName, out _);

                throw;
            }
        }

        void Remove(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            _endpoints.TryRemove(endpointName, out _);
        }


        class Handle :
            HostReceiveEndpointHandle
        {
            readonly ReceiveEndpoint _endpoint;
            readonly Action _remove;

            ReceiveEndpointHandle _endpointHandle;

            public Handle(ReceiveEndpoint endpoint, Action remove)
            {
                _endpoint = endpoint;
                _remove = remove;
            }

            public IReceiveEndpoint ReceiveEndpoint => _endpoint;

            public Task<ReceiveEndpointReady> Ready => _endpointHandle.Ready;

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _remove();

                return _endpoint.Stop(true, cancellationToken);
            }

            public void Start(CancellationToken cancellationToken)
            {
                if (_endpointHandle != null)
                    return;

                _endpointHandle = _endpoint.Start(cancellationToken);
            }
        }
    }
}
