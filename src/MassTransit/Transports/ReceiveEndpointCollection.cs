namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline.Observables;
    using Util;


    public class ReceiveEndpointCollection :
        Agent,
        IReceiveEndpointCollection
    {
        readonly SingleThreadedDictionary<string, ReceiveEndpoint> _endpoints;
        readonly ReceiveEndpointStateMachine _machine;
        readonly ReceiveEndpointObservable _receiveEndpointObservers;
        bool _started;

        public ReceiveEndpointCollection()
        {
            _machine = new ReceiveEndpointStateMachine();

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

            var added = _endpoints.TryAdd(endpointName, key =>
            {
                endpoint.ConnectReceiveEndpointObserver(new ReceiveEndpointStateMachineObserver(_machine, endpoint));

                return endpoint;
            });

            if (!added)
                throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");
        }

        public HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
        {
            _started = true;

            KeyValuePair<string, ReceiveEndpoint>[] endpointsToStart = _endpoints.Where(x => !_machine.IsStarted(x.Value)).ToArray();

            return endpointsToStart.Select(x => StartEndpoint(x.Key, x.Value, cancellationToken)).ToArray();
        }

        public HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            if (!_endpoints.TryGetValue(endpointName, out var endpoint))
                throw new ConfigurationException($"A receive endpoint with the key was not found: {endpointName}");

            if (_machine.IsStarted(endpoint))
                throw new ArgumentException($"The specified endpoint has already been started: {endpointName}", nameof(endpointName));

            return StartEndpoint(endpointName, endpoint, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            foreach (KeyValuePair<string, ReceiveEndpoint> endpoint in _endpoints)
            {
                var endpointScope = context.CreateScope("receiveEndpoint");
                endpointScope.Add("name", endpoint.Key);
                if (_machine.IsStarted(endpoint.Value))
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

        public HealthResult CheckHealth(BusState busState, string healthMessage)
        {
            var results = _endpoints.Values.Select(x => new
            {
                x.InputAddress,
                Result = x.HealthResult
            }).ToArray();

            var unhealthy = results.Where(x => x.Result.Status == BusHealthStatus.Unhealthy).ToArray();
            var degraded = results.Where(x => x.Result.Status == BusHealthStatus.Degraded).ToArray();

            var unhappy = unhealthy.Union(degraded).ToArray();

            var names = unhappy.Select(x => x.InputAddress.AbsolutePath.Split('/').LastOrDefault()).ToArray();

            Dictionary<string, EndpointHealthResult> data = results.ToDictionary(x => x.InputAddress.ToString(), x => x.Result);

            var exception = results.Where(x => x.Result.Exception != null).Select(x => x.Result.Exception).FirstOrDefault();

            if (busState != BusState.Started || unhealthy.Any() && unhappy.Length == results.Length)
                return HealthResult.Unhealthy($"Not ready: {healthMessage}", exception, data);

            if (degraded.Any())
                return HealthResult.Degraded($"Degraded Endpoints: {string.Join(",", names)}", exception, data);

            return HealthResult.Healthy("Ready", data);
        }

        protected override async Task StopAgent(StopContext context)
        {
            ReceiveEndpoint[] endpoints = _endpoints.Values.Where(x => _machine.IsStarted(x)).ToArray();

            await Task.WhenAll(endpoints.Select(x => x.Stop(context.CancellationToken))).ConfigureAwait(false);

            _started = false;

            await base.StopAgent(context).ConfigureAwait(false);
        }

        HostReceiveEndpointHandle StartEndpoint(string endpointName, ReceiveEndpoint endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var observerHandle = endpoint.ConnectReceiveEndpointObserver(_receiveEndpointObservers);

                void RemoveEndpoint()
                {
                    observerHandle.Disconnect();
                    Remove(endpointName);
                }

                var handle = new Handle(endpoint, RemoveEndpoint);

                handle.Start(cancellationToken);

                TaskUtil.Await(() => _machine.RaiseEvent(endpoint, _machine.ReceiveEndpointStarted, cancellationToken), cancellationToken);

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

            Task<ReceiveEndpointReady> HostReceiveEndpointHandle.Ready => _endpointHandle.Ready;

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                await _endpoint.Stop(true, cancellationToken).ConfigureAwait(false);

                _remove();
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
