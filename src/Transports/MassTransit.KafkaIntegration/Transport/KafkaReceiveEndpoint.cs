namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Events;
    using GreenPipes;
    using Pipeline;
    using Transports;
    using Util;


    public class KafkaReceiveEndpoint<TKey, TValue> :
        IKafkaReceiveEndpoint
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly ReceiveEndpointContext _context;
        readonly ChannelExecutor _executor;
        readonly TaskCompletionSource<ReceiveEndpointReady> _started;
        readonly string _topic;
        readonly IKafkaReceiveTransport<TKey, TValue> _transport;
        CancellationTokenSource _cancellationTokenSource;
        Task _consumerTask;

        public KafkaReceiveEndpoint(string topic, int prefetch, int concurrencyLimit, IConsumer<TKey, TValue> consumer,
            IKafkaReceiveTransport<TKey, TValue> transport,
            ReceiveEndpointContext context)
        {
            _topic = topic;
            _consumer = consumer;
            _transport = transport;
            _context = context;

            _started = TaskUtil.GetTask<ReceiveEndpointReady>();
            _executor = new ChannelExecutor(prefetch, concurrencyLimit);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
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
            return _context.ReceivePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _context.ReceivePipe.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
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

        public Task Connect(CancellationToken cancellationToken)
        {
            var logContext = _context.LogContext;
            LogContext.SetCurrentIfNull(logContext);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // TODO this feels like it needs to be more resilient, right now one error and it exits

            _consumerTask = Task.Run(async () =>
            {
                var inputAddress = _context.InputAddress;
                await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

                var endpointReadyEvent = new ReceiveEndpointReadyEvent(_context.InputAddress, this, true);
                _started.TrySetResult(endpointReadyEvent);

                await _context.EndpointObservers.Ready(endpointReadyEvent).ConfigureAwait(false);

                LogContext.Debug?.Log("Kafka consumer started: {InputAddress}", _context.InputAddress);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    LogContext.SetCurrentIfNull(logContext);
                    try
                    {
                        ConsumeResult<TKey, TValue> message = _consumer.Consume(_cancellationTokenSource.Token);

                        await _executor.Push(() => _transport.Handle(message, cancellationToken), _cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException e) when (e.CancellationToken == _cancellationTokenSource.Token)
                    {
                    }
                    catch (Exception exception)
                    {
                        var faultedEvent = new ReceiveTransportFaultedEvent(inputAddress, exception);

                        await _context.TransportObservers.Faulted(faultedEvent).ConfigureAwait(false);
                        await _context.EndpointObservers.Faulted(new ReceiveEndpointFaultedEvent(faultedEvent, this)).ConfigureAwait(false);

                        throw;
                    }
                }
            }, cancellationToken);

            LogContext.Debug?.Log("Kafka consumer starting: {Topic}", _topic);

            _consumer.Subscribe(_topic);

            return _consumerTask.IsCompleted ? _consumerTask : TaskUtil.Completed;
        }

        public async Task Disconnect(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);
            try
            {
                _cancellationTokenSource.Cancel();

                await _consumerTask.ConfigureAwait(false);
                await _executor.DisposeAsync().ConfigureAwait(false);

                _consumer.Close();
                _consumer.Dispose();

                _cancellationTokenSource.Dispose();
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "Error occured while stopping kafka topic: '{topicName}' listener", _topic);
            }
            finally
            {
                var metrics = _transport.GetMetrics();
                var completedEvent = new ReceiveTransportCompletedEvent(_context.InputAddress, metrics);

                await _context.TransportObservers.Completed(completedEvent).ConfigureAwait(false);
                await _context.EndpointObservers.Completed(new ReceiveEndpointCompletedEvent(completedEvent, this)).ConfigureAwait(false);

                LogContext.Debug?.Log("Kafka consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    _context.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }

        public void Probe(ProbeContext context)
        {
            _transport.Probe(context);
            _context.ReceivePipe.Probe(context);
        }

        public Task<ReceiveEndpointReady> Started => _started.Task;
    }
}
