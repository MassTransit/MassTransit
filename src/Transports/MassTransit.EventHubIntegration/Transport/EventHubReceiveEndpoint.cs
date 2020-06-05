namespace MassTransit.EventHubIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Context;
    using Events;
    using GreenPipes;
    using Util;


    public class EventHubReceiveEndpoint :
        IEventHubReceiveEndpoint
    {
        readonly BlobContainerClient _blobContainerClient;
        readonly ReceiveEndpointContext _context;
        readonly ChannelExecutor _executor;
        readonly EventProcessorClient _processor;
        readonly TaskCompletionSource<ReceiveEndpointReady> _started;
        readonly IEventHubDataReceiver _transport;
        CancellationTokenSource _cancellationTokenSource;

        public EventHubReceiveEndpoint(EventProcessorClient processor, int prefetch, int concurrencyLimit, BlobContainerClient blobContainerClient,
            IEventHubDataReceiver transport, ReceiveEndpointContext context)
        {
            _processor = processor;
            _blobContainerClient = blobContainerClient;
            _transport = transport;
            _context = context;

            _started = TaskUtil.GetTask<ReceiveEndpointReady>();
            _executor = new ChannelExecutor(prefetch, concurrencyLimit);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.SendEndpointProvider.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return _context.PublishEndpointProvider.GetPublishSendEndpoint<T>();
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

        public void Probe(ProbeContext context)
        {
            _transport.Probe(context);
            _context.ReceivePipe.Probe(context);
        }

        public Task<ReceiveEndpointReady> Started => _started.Task;

        public async Task Connect(CancellationToken cancellationToken)
        {
            var logContext = _context.LogContext;
            var inputAddress = _context.InputAddress;
            LogContext.SetCurrentIfNull(logContext);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            async Task ProcessEventAsync(ProcessEventArgs arg)
            {
                LogContext.SetCurrentIfNull(logContext);

                try
                {
                    await _executor.Push(() => _transport.Handle(arg, cancellationToken), _cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException e) when (e.CancellationToken == _cancellationTokenSource.Token)
                {
                }
            }

            async Task ProcessErrorAsync(ProcessErrorEventArgs arg)
            {
                var faultedEvent = new ReceiveTransportFaultedEvent(inputAddress, arg.Exception);

                await _context.TransportObservers.Faulted(faultedEvent).ConfigureAwait(false);
                await _context.EndpointObservers.Faulted(new ReceiveEndpointFaultedEvent(faultedEvent, this)).ConfigureAwait(false);
            }

            _processor.ProcessEventAsync += ProcessEventAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            try
            {
                await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException exception)
            {
                LogContext.Warning?.Log(exception, "Azure Blob Container does not exist: {Address}", _blobContainerClient.Uri);
            }

            LogContext.Debug?.Log("EventHub processor starting: {EventHubName}", _processor.EventHubName);

            await _processor.StartProcessingAsync(cancellationToken).ConfigureAwait(false);

            await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            var endpointReadyEvent = new ReceiveEndpointReadyEvent(_context.InputAddress, this, true);
            _started.TrySetResult(endpointReadyEvent);

            await _context.EndpointObservers.Ready(endpointReadyEvent).ConfigureAwait(false);
        }

        public async Task Disconnect(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            try
            {
                _cancellationTokenSource.Cancel();

                await _executor.DisposeAsync().ConfigureAwait(false);
                await _processor.StopProcessingAsync(cancellationToken).ConfigureAwait(false);

                _cancellationTokenSource.Dispose();
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "Error occured while stopping EventHub processor: '{EventHubName}' listener", _processor.EventHubName);
            }
            finally
            {
                var metrics = _transport.GetMetrics();
                var completedEvent = new ReceiveTransportCompletedEvent(_context.InputAddress, metrics);

                await _context.TransportObservers.Completed(completedEvent).ConfigureAwait(false);
                await _context.EndpointObservers.Completed(new ReceiveEndpointCompletedEvent(completedEvent, this)).ConfigureAwait(false);

                LogContext.Debug?.Log("EventHub processor completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }
    }
}
