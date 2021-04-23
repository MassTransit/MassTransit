namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Metrics;


    public class GrpcQueue :
        IGrpcQueue
    {
        readonly CancellationTokenSource _cancel;
        readonly Channel<DeliveryContext<GrpcTransportMessage>> _channel;
        readonly ConsumerCollection _consumers;
        readonly Task _dispatcher;
        readonly QueueMetric _metrics;
        readonly string _name;
        readonly IMessageFabricObserver _observer;

        public GrpcQueue(IMessageFabricObserver observer, string name)
        {
            _observer = observer;
            _name = name;

            _cancel = new CancellationTokenSource();
            _consumers = new ConsumerCollection(consumers => new RoundRobinConsumerLoadBalancer(consumers));
            _metrics = new QueueMetric(name);

            _channel = Channel.CreateUnbounded<DeliveryContext<GrpcTransportMessage>>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            });

            _dispatcher = Task.Run(() => StartDispatcher());
        }

        public TopologyHandle ConnectConsumer(NodeContext nodeContext, IGrpcQueueConsumer consumer)
        {
            try
            {
                var handle = _consumers.Connect(consumer);

                handle = _observer.ConsumerConnected(nodeContext, handle, _name);

                return handle;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {_name}", exception);
            }
        }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            if (context.WasAlreadyDelivered(this))
                return;

            if (context.Message.EnqueueTime.HasValue)
                DeliverWithDelay(context);
            else
            {
                await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);

                _metrics.MessageCount.Add();
            }
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public async ValueTask DisposeAsync()
        {
            _cancel.Cancel();

            _channel.Writer.Complete();

            await _channel.Reader.Completion.ConfigureAwait(false);

            _cancel.Cancel();

            await _dispatcher.ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("queue");
            scope.Add("name", _name);
        }

        void DeliverWithDelay(DeliveryContext<GrpcTransportMessage> context)
        {
            Task.Run(async () =>
            {
                var delayed = false;
                try
                {
                    var delay = context.Message.EnqueueTime.Value - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        _metrics.DelayedMessageCount.Add();
                        delayed = true;

                        await Task.Delay(delay, context.CancellationToken).ConfigureAwait(false);
                    }

                    await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);
                    _metrics.MessageCount.Add();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "Message delivery faulted: {Queue}", _name);
                }
                finally
                {
                    if (delayed)
                        _metrics.DelayedMessageCount.Remove();
                }
            }, context.CancellationToken);
        }

        async Task StartDispatcher()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cancel.Token).ConfigureAwait(false))
                {
                    DeliveryContext<GrpcTransportMessage> context = await _channel.Reader.ReadAsync(_cancel.Token).ConfigureAwait(false);
                    _metrics.MessageCount.Remove();

                    try
                    {
                        IGrpcQueueConsumer consumer = null;
                        if (context.Message.Message.Deliver.DestinationCase == Contracts.Deliver.DestinationOneofCase.Consumer)
                            _consumers.TryGetConsumer(context.Message.Message.Deliver.Consumer.ConsumerId, out consumer);

                        consumer ??= await _consumers.Next(context.Message, _cancel.Token).ConfigureAwait(false);
                        if (consumer != null)
                            await consumer.Consume(context.Message, _cancel.Token).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Warning?.Log(exception, "Failed to dispatch message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Queue dispatcher faulted: {Queue}", _name);
            }
        }

        public override string ToString()
        {
            return $"Queue({_name})";
        }
    }
}
