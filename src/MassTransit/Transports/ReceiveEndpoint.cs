namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using Util;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context.
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpoint
    {
        public enum State
        {
            Initial,
            Started,
            Ready,
            Completed,
            Faulted,
            Final
        }


        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<ReceiveEndpointReady> _started;
        readonly StartObserver _startObserver;
        readonly IReceiveTransport _transport;
        EndpointHandle _handle;

        public ReceiveEndpoint(IReceiveTransport transport, ReceiveEndpointContext context)
        {
            _context = context;
            _transport = transport;

            _started = new TaskCompletionSource<ReceiveEndpointReady>(TaskCreationOptions.RunContinuationsAsynchronously);

            InputAddress = context.InputAddress;

            _startObserver = new StartObserver();

            ConnectReceiveEndpointObserver(_startObserver);

            transport.ConnectReceiveTransportObserver(new Observer(this, context.EndpointObservers));
        }

        public State CurrentState { get; set; }

        public string Message { get; set; }

        public EndpointHealthResult HealthResult { get; set; }

        public Uri InputAddress { get; set; }

        public Task<ReceiveEndpointReady> Started => _started.Task;

        public ReceiveEndpointHandle Start(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_handle != null)
                throw new InvalidOperationException($"The receive endpoint was already started: {InputAddress}");

            _handle = new EndpointHandle(this, _transport, _startObserver, cancellationToken);

            _handle.Start();

            switch (CurrentState)
            {
                case State.Initial:
                case State.Completed:
                case State.Faulted:
                    Message = "starting";
                    CurrentState = State.Started;
                    break;
            }

            return _handle;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return Stop(false, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            _transport.Probe(context);

            _context.ReceivePipe.Probe(context);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _context.ReceivePipe.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.SendEndpointProvider.GetSendEndpoint(address);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return _context.PublishEndpointProvider.GetPublishSendEndpoint<T>();
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _context.ConnectReceiveEndpointObserver(observer);
        }

        public bool IsStarted()
        {
            return State.Started.Equals(CurrentState) || State.Ready.Equals(CurrentState) || State.Faulted.Equals(CurrentState);
        }

        public async Task Stop(bool removed, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_handle != null)
            {
                await _context.EndpointObservers.Stopping(new ReceiveEndpointStoppingEvent(_context.InputAddress, this, removed)).ConfigureAwait(false);

                await _handle.TransportHandle.Stop(cancellationToken).ConfigureAwait(false);

                _handle = null;
            }

            _context.Reset();
        }


        class Observer :
            IReceiveTransportObserver
        {
            readonly ReceiveEndpoint _endpoint;
            readonly IReceiveEndpointObserver _observer;

            public Observer(ReceiveEndpoint endpoint, IReceiveEndpointObserver observer)
            {
                _endpoint = endpoint;
                _observer = observer;
            }

            public Task Ready(ReceiveTransportReady ready)
            {
                var endpointReadyEvent = new ReceiveEndpointReadyEvent(ready.InputAddress, _endpoint, ready.IsStarted);
                if (ready.IsStarted)
                    _endpoint._started.TrySetResult(endpointReadyEvent);

                return _observer.Ready(endpointReadyEvent);
            }

            public Task Completed(ReceiveTransportCompleted completed)
            {
                return _observer.Completed(new ReceiveEndpointCompletedEvent(completed, _endpoint));
            }

            public Task Faulted(ReceiveTransportFaulted faulted)
            {
                return _observer.Faulted(new ReceiveEndpointFaultedEvent(faulted, _endpoint));
            }
        }


        class StartObserver :
            IReceiveEndpointObserver
        {
            readonly Connectable<EndpointHandle> _handles;

            public StartObserver()
            {
                _handles = new Connectable<EndpointHandle>();
            }

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                return _handles.ForEachAsync(x => x.SetReady(ready));
            }

            public Task Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                return _handles.ForEachAsync(x => x.SetFaulted(faulted));
            }

            public ConnectHandle ConnectEndpointHandle(EndpointHandle handle)
            {
                return _handles.Connect(handle);
            }
        }


        class EndpointHandle :
            ReceiveEndpointHandle
        {
            readonly CancellationToken _cancellationToken;
            readonly ReceiveEndpoint _endpoint;
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<ReceiveEndpointReady> _ready;
            readonly IReceiveTransport _transport;
            ReceiveEndpointFaulted _faulted;
            CancellationTokenRegistration _registration;

            public EndpointHandle(ReceiveEndpoint endpoint, IReceiveTransport transport, StartObserver startObserver, CancellationToken cancellationToken)
            {
                _endpoint = endpoint;
                _transport = transport;

                _cancellationToken = cancellationToken;
                _ready = new TaskCompletionSource<ReceiveEndpointReady>(TaskCreationOptions.RunContinuationsAsynchronously);

                if (cancellationToken.CanBeCanceled)
                {
                    _registration = cancellationToken.Register(() =>
                    {
                        if (_faulted != null)
                        {
                            _handle?.Disconnect();
                            _ready.TrySetException(_faulted.Exception);
                        }

                        _registration.Dispose();
                    });
                }

                _handle = startObserver.ConnectEndpointHandle(this);
            }

            public ReceiveTransportHandle TransportHandle { get; private set; }

            public Task<ReceiveEndpointReady> Ready => _ready.Task;

            Task ReceiveEndpointHandle.Stop(CancellationToken cancellationToken)
            {
                return _endpoint.Stop(cancellationToken);
            }

            public void Start()
            {
                TransportHandle = _transport.Start();
            }

            public Task SetReady(ReceiveEndpointReady ready)
            {
                _handle.Disconnect();
                _registration.Dispose();

                _ready.TrySetResult(ready);

                return Task.CompletedTask;
            }

            public Task SetFaulted(ReceiveEndpointFaulted faulted)
            {
                _faulted = faulted;

                if (_cancellationToken.IsCancellationRequested || IsUnrecoverable(faulted.Exception))
                {
                    _handle.Disconnect();
                    _registration.Dispose();

                    _ready.TrySetException(faulted.Exception);
                }

                return Task.CompletedTask;
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
