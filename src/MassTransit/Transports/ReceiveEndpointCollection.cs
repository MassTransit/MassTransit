namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Pipeline.Observables;
    using Util;


    public class ReceiveEndpointCollection :
        Agent,
        IReceiveEndpointCollection
    {
        readonly ConsumeObservable _consumeObservers;
        readonly Dictionary<string, IReceiveEndpointControl> _endpoints;
        readonly Dictionary<string, HostReceiveEndpointHandle> _handles;
        readonly object _mutateLock = new object();
        readonly PublishObservable _publishObservers;
        readonly ReceiveEndpointObservable _receiveEndpointObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;

        public ReceiveEndpointCollection()
        {
            _endpoints = new Dictionary<string, IReceiveEndpointControl>(StringComparer.OrdinalIgnoreCase);
            _handles = new Dictionary<string, HostReceiveEndpointHandle>(StringComparer.OrdinalIgnoreCase);
            _receiveObservers = new ReceiveObservable();
            _receiveEndpointObservers = new ReceiveEndpointObservable();
            _consumeObservers = new ConsumeObservable();
            _publishObservers = new PublishObservable();
            _sendObservers = new SendObservable();
        }

        public void Add(string endpointName, IReceiveEndpointControl endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            lock (_mutateLock)
            {
                if (_endpoints.ContainsKey(endpointName))
                    throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");

                _endpoints.Add(endpointName, endpoint);
            }
        }

        public HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken)
        {
            KeyValuePair<string, IReceiveEndpointControl>[] startable;
            lock (_mutateLock)
                startable = _endpoints.Where(x => !_handles.ContainsKey(x.Key)).ToArray();

            return startable.Select(x => StartEndpoint(x.Key, x.Value, cancellationToken)).ToArray();
        }

        public HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            IReceiveEndpointControl endpoint;
            lock (_mutateLock)
            {
                if (!_endpoints.TryGetValue(endpointName, out endpoint))
                    throw new ConfigurationException($"A receive endpoint with the key was not found: {endpointName}");

                if (_handles.ContainsKey(endpointName))
                    throw new ArgumentException($"The specified endpoint has already been started: {endpointName}", nameof(endpointName));
            }

            return StartEndpoint(endpointName, endpoint, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            foreach (KeyValuePair<string, IReceiveEndpointControl> receiveEndpoint in _endpoints)
            {
                var endpointScope = context.CreateScope("receiveEndpoint");
                endpointScope.Add("name", receiveEndpoint.Key);
                if (_handles.ContainsKey(receiveEndpoint.Key))
                    endpointScope.Add("started", true);

                receiveEndpoint.Value.Probe(endpointScope);
            }
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
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

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _consumeObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        protected override async Task StopAgent(StopContext context)
        {
            HostReceiveEndpointHandle[] handles;
            lock (_mutateLock)
                handles = _handles.Values.ToArray();

            await Task.WhenAll(handles.Select(x => x.StopAsync(context.CancellationToken))).ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);
        }

        HostReceiveEndpointHandle StartEndpoint(string endpointName, IReceiveEndpointControl endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var endpointReady = new ReceiveEndpointReadyObserver(endpoint, cancellationToken);

                var consumeObserver = endpoint.ConnectConsumeObserver(_consumeObservers);
                var receiveObserver = endpoint.ConnectReceiveObserver(_receiveObservers);
                var receiveEndpointObserver = endpoint.ConnectReceiveEndpointObserver(_receiveEndpointObservers);
                var publishObserver = endpoint.ConnectPublishObserver(_publishObservers);
                var sendObserver = endpoint.ConnectSendObserver(_sendObservers);
                var endpointHandle = endpoint.Start();

                var handle = new Handle(endpointHandle, endpoint, endpointReady.Ready, () => Remove(endpointName),
                    receiveObserver, receiveEndpointObserver, consumeObserver, publishObserver, sendObserver);

                lock (_mutateLock)
                    _handles.Add(endpointName, handle);

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
            {
                _endpoints.Remove(endpointName);
                _handles.Remove(endpointName);
            }
        }


        class Handle :
            HostReceiveEndpointHandle
        {
            readonly ReceiveEndpointHandle _endpointHandle;
            readonly ConnectHandle[] _handles;
            readonly Action _onStopped;
            bool _stopped;

            public Handle(ReceiveEndpointHandle endpointHandle, IReceiveEndpoint receiveEndpoint, Task<ReceiveEndpointReady> ready, Action onStopped,
                params ConnectHandle[] handles)
            {
                _endpointHandle = endpointHandle;
                _onStopped = onStopped;
                _handles = handles;
                ReceiveEndpoint = receiveEndpoint;

                Ready = ready;
            }

            public IReceiveEndpoint ReceiveEndpoint { get; }
            public Task<ReceiveEndpointReady> Ready { get; }

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                foreach (var handle in _handles)
                    handle.Disconnect();

                await _endpointHandle.Stop(cancellationToken).ConfigureAwait(false);

                _onStopped();

                _stopped = true;
            }
        }


        class ReceiveEndpointReadyObserver
        {
            readonly Observer _observer;

            public ReceiveEndpointReadyObserver(IReceiveEndpoint receiveEndpoint, CancellationToken cancellationToken)
            {
                _observer = new Observer(receiveEndpoint, cancellationToken);
            }

            public Task<ReceiveEndpointReady> Ready => _observer.Ready;


            class Observer :
                IReceiveEndpointObserver
            {
                readonly CancellationToken _cancellationToken;
                readonly ConnectHandle _handle;
                readonly TaskCompletionSource<ReceiveEndpointReady> _ready;
                ReceiveEndpointFaulted _faulted;
                CancellationTokenRegistration _registration;

                public Observer(IReceiveEndpointObserverConnector endpoint, CancellationToken cancellationToken)
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

                static bool IsUnrecoverable(Exception exception) =>
                    exception switch
                    {
                        ConnectionException connectionException => !connectionException.IsTransient,
                        _ => false
                    };
            }
        }
    }
}
