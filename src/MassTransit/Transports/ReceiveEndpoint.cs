namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using Events;
    using GreenPipes;
    using Monitoring.Health;
    using Util;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context.
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpoint
    {
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<ReceiveEndpointReady> _started;
        readonly IReceiveTransport _transport;
        EndpointHandle _handle;

        public ReceiveEndpoint(IReceiveTransport transport, ReceiveEndpointContext context)
        {
            _context = context;
            _transport = transport;

            _started = TaskUtil.GetTask<ReceiveEndpointReady>();

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

            var endpointReadyObserver = new StartEndpointReadyObserver(this, cancellationToken);

            var transportHandle = _transport.Start();

            _handle = new EndpointHandle(this, transportHandle, endpointReadyObserver);

            return _handle;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_handle == null)
                return;

            await _context.EndpointObservers.Stopping(new ReceiveEndpointStoppingEvent(_context.InputAddress, this)).ConfigureAwait(false);

            await _handle.TransportHandle.Stop(cancellationToken).ConfigureAwait(false);

            _context.Reset();

            _handle = null;
        }

        public void Probe(ProbeContext context)
        {
            _transport.Probe(context);

            _context.ReceivePipe.Probe(context);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _context.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumePipe(pipe);
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

        public EndpointHealthResult CheckHealth()
        {
            return HealthResult;
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
                            _ready.TrySetException(_faulted.Exception);
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

                _ready.TrySetResult(ready);

                return TaskUtil.Completed;
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
                    _registration.Dispose();

                    _ready.TrySetException(faulted.Exception);
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


        class EndpointHandle :
            ReceiveEndpointHandle
        {
            readonly ReceiveEndpoint _endpoint;
            readonly StartEndpointReadyObserver _observer;

            public EndpointHandle(ReceiveEndpoint endpoint, ReceiveTransportHandle transportHandle, StartEndpointReadyObserver observer)
            {
                _endpoint = endpoint;
                _observer = observer;

                TransportHandle = transportHandle;
            }

            public ReceiveTransportHandle TransportHandle { get; }

            public Task<ReceiveEndpointReady> Ready => _observer.Ready;

            Task ReceiveEndpointHandle.Stop(CancellationToken cancellationToken)
            {
                return _endpoint.Stop(cancellationToken);
            }
        }
    }
}
