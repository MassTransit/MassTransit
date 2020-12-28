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
        readonly Dictionary<string, ReceiveEndpoint> _endpoints;
        readonly ReceiveEndpointStateMachine _machine;
        readonly object _mutateLock = new object();
        readonly ReceiveEndpointObservable _receiveEndpointObservers;

        public ReceiveEndpointCollection()
        {
            _machine = new ReceiveEndpointStateMachine();

            _receiveEndpointObservers = new ReceiveEndpointObservable();

            _endpoints = new Dictionary<string, ReceiveEndpoint>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string endpointName, ReceiveEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            lock (_mutateLock)
            {
                if (_endpoints.ContainsKey(endpointName))
                    throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");

                endpoint.ConnectReceiveEndpointObserver(new ReceiveEndpointStateMachineObserver(_machine, endpoint));

                _endpoints.Add(endpointName, endpoint);
            }
        }

        public HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
        {
            KeyValuePair<string, ReceiveEndpoint>[] endpointsToStart;
            lock (_mutateLock)
                endpointsToStart = _endpoints.Where(x => !_machine.IsStarted(x.Value)).ToArray();

            return endpointsToStart.Select(x => StartEndpoint(x.Key, x.Value, cancellationToken)).ToArray();
        }

        public HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            ReceiveEndpoint endpoint;
            lock (_mutateLock)
            {
                if (!_endpoints.TryGetValue(endpointName, out endpoint))
                    throw new ConfigurationException($"A receive endpoint with the key was not found: {endpointName}");

                if (_machine.IsStarted(endpoint))
                    throw new ArgumentException($"The specified endpoint has already been started: {endpointName}", nameof(endpointName));
            }

            return StartEndpoint(endpointName, endpoint, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            lock (_mutateLock)
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
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            lock (_mutateLock)
                return new MultipleConnectHandle(_endpoints.Values.Select(x => x.ConnectConsumeMessageObserver(observer)));
        }

        protected override async Task StopAgent(StopContext context)
        {
            ReceiveEndpoint[] endpoints;
            lock (_mutateLock)
                endpoints = _endpoints.Values.Where(x => _machine.IsStarted(x)).ToArray();

            await Task.WhenAll(endpoints.Select(x => x.Stop(context.CancellationToken))).ConfigureAwait(false);

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
                lock (_mutateLock)
                    _endpoints.Remove(endpointName);

                throw;
            }
        }

        void Remove(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            lock (_mutateLock)
                _endpoints.Remove(endpointName);
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
                await _endpoint.Stop(cancellationToken).ConfigureAwait(false);

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
