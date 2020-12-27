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
    using GreenPipes.Internals.Extensions;
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

            await Task.WhenAll(endpoints.Select(x => x.EndpointHandle.StopAsync(false, context.CancellationToken))).ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);
        }

        HostReceiveEndpointHandle StartEndpoint(string endpointName, ReceiveEndpoint endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var endpointReadyObserver = new StartEndpointReadyObserver(endpoint, cancellationToken);

                var observerHandle = endpoint.ConnectReceiveEndpointObserver(_receiveEndpointObservers);

                var endpointHandle = endpoint.Start(cancellationToken);

                HostReceiveEndpointHandle handle = new Handle(endpointHandle, endpoint, endpointReadyObserver.Ready, () => Remove(endpointName),
                    observerHandle);

                TaskUtil.Await(() => _machine.RaiseEvent(endpoint, x => x.ReceiveEndpointStarted, handle, cancellationToken), cancellationToken);

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
            readonly ReceiveEndpointHandle _endpointHandle;
            readonly ConnectHandle[] _handles;
            readonly Action _remove;
            bool _stopped;

            public Handle(ReceiveEndpointHandle endpointHandle, ReceiveEndpoint endpoint, Task<ReceiveEndpointReady> ready, Action remove,
                params ConnectHandle[] handles)
            {
                _endpoint = endpoint;
                _endpointHandle = endpointHandle;
                _remove = remove;
                _handles = handles;

                Ready = ready;
            }

            public IReceiveEndpoint ReceiveEndpoint => _endpoint;
            public Task<ReceiveEndpointReady> Ready { get; }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return StopAsync(true, cancellationToken);
            }

            public async Task StopAsync(bool removeEndpoint, CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                await _endpointHandle.Stop(cancellationToken).ConfigureAwait(false);

                foreach (var handle in _handles)
                    handle.Disconnect();

                if (removeEndpoint)
                    _remove();

                _stopped = true;
            }
        }


        class StartEndpointReadyObserver :
            IReceiveEndpointObserver
        {
            readonly CancellationToken _cancellationToken;
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<ReceiveEndpointReady> _ready;
            ReceiveEndpointFaulted _faulted;
            CancellationTokenRegistration _registration;

            public StartEndpointReadyObserver(IReceiveEndpointObserverConnector endpoint, CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
                _ready = TaskUtil.GetTask<ReceiveEndpointReady>();

                if (cancellationToken.CanBeCanceled)
                {
                    _registration = cancellationToken.Register(() =>
                    {
                        if (_faulted != null)
                        {
                            _handle?.Disconnect();
                            _ready.TrySetExceptionOnThreadPool(_faulted.Exception);
                        }

                        _registration.Dispose();
                    });
                }

                _handle = endpoint.ConnectReceiveEndpointObserver(this);
            }

            public Task<ReceiveEndpointReady> Ready => _ready.Task;

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                _handle.Disconnect();
                _registration.Dispose();

                return _ready.TrySetResultOnThreadPool(ready);
            }

            public Task Stopping(ReceiveEndpointStopping stopping)
            {
                return TaskUtil.Completed;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                return TaskUtil.Completed;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                _faulted = faulted;

                if (_cancellationToken.IsCancellationRequested || IsUnrecoverable(faulted.Exception))
                {
                    _handle.Disconnect();

                    return _ready.TrySetExceptionOnThreadPool(faulted.Exception);
                }

                return TaskUtil.Completed;
            }

            static bool IsUnrecoverable(Exception exception)
            {
                return exception switch
                {
                    ConnectionException connectionException => !connectionException.IsTransient,
                    _ => false
                };
            }
        }
    }
}
