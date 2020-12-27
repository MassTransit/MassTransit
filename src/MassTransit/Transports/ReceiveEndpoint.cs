namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
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

        public ReceiveEndpoint(IReceiveTransport transport, ReceiveEndpointContext context)
        {
            _context = context;
            _transport = transport;

            _started = TaskUtil.GetTask<ReceiveEndpointReady>();

            transport.ConnectReceiveTransportObserver(new Observer(this, context.EndpointObservers));
        }

        public State CurrentState { get; set; }

        public HostReceiveEndpointHandle EndpointHandle { get; set; }

        public string Message { get; set; }

        public HealthResult HealthResult { get; set; }

        public Uri InputAddress { get; set; }

        public Task<ReceiveEndpointReady> Started => _started.Task;

        public ReceiveEndpointHandle Start(CancellationToken cancellationToken)
        {
            var transportHandle = _transport.Start();

            return new Handle(this, transportHandle, _context);
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

        public HealthResult CheckHealth()
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
                    _endpoint._started.TrySetResultOnThreadPool(endpointReadyEvent);

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


        class Handle :
            ReceiveEndpointHandle
        {
            readonly ReceiveEndpointContext _context;
            readonly ReceiveEndpoint _endpoint;
            readonly ReceiveTransportHandle _transportHandle;

            public Handle(ReceiveEndpoint endpoint, ReceiveTransportHandle transportHandle, ReceiveEndpointContext context)
            {
                _endpoint = endpoint;
                _transportHandle = transportHandle;
                _context = context;
            }

            async Task ReceiveEndpointHandle.Stop(CancellationToken cancellationToken)
            {
                await _context.EndpointObservers.Stopping(new ReceiveEndpointStoppingEvent(_context.InputAddress, _endpoint)).ConfigureAwait(false);

                await _transportHandle.Stop(cancellationToken).ConfigureAwait(false);

                _context.Reset();
            }
        }
    }
}
