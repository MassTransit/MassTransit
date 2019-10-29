namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Pipeline;
    using Util;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context.
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpointControl
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceiveTransport _receiveTransport;
        readonly TaskCompletionSource<ReceiveEndpointReady> _started;
        ConnectHandle _handle;

        public ReceiveEndpoint(IReceiveTransport receiveTransport, ReceiveEndpointContext context)
        {
            _context = context;
            _receiveTransport = receiveTransport;

            _started = TaskUtil.GetTask<ReceiveEndpointReady>();
            _handle = receiveTransport.ConnectReceiveTransportObserver(new Observer(this, context.EndpointObservers));
        }

        public Task<ReceiveEndpointReady> Started => _started.Task;

        ReceiveEndpointHandle IReceiveEndpointControl.Start()
        {
            var transportHandle = _receiveTransport.Start();

            return new Handle(this, transportHandle, _context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _receiveTransport.Probe(context);

            _context.ReceivePipe.Probe(context);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveTransport.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _context.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            IPipe<ConsumeContext<T>> messagePipe = _context.ConsumePipeSpecification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            return _context.ReceivePipe.ConnectConsumePipe(messagePipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            IPipe<ConsumeContext<T>> messagePipe = _context.ConsumePipeSpecification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            return _context.ReceivePipe.ConnectRequestPipe(requestId, messagePipe);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveTransport.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveTransport.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.SendEndpointProvider.GetSendEndpoint(address);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, ConsumeContext context = null)
        {
            return _context.PublishEndpointProvider.CreatePublishEndpoint(sourceAddress, context);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class
        {
            return _context.PublishEndpointProvider.GetPublishSendEndpoint(message);
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
                {
                    _endpoint._started.TrySetResultOnThreadPool(endpointReadyEvent);
                }

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
            readonly ReceiveEndpoint _endpoint;
            readonly ReceiveTransportHandle _transportHandle;
            readonly ReceiveEndpointContext _context;

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
            }
        }
    }
}
