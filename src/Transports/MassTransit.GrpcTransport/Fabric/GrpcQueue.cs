namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Util;


    public class GrpcQueue :
        IGrpcQueue
    {
        readonly Channel<DeliveryContext<GrpcTransportMessage>> _channel;
        readonly TaskCompletionSource<IGrpcQueueConsumer> _consumer;
        readonly ConsumerCollection<IGrpcQueueConsumer> _consumers;
        readonly CancellationTokenSource _cancel;
        readonly Task _dispatcher;
        readonly string _name;
        readonly IMessageFabricObserver _observer;

        public GrpcQueue(IMessageFabricObserver observer, string name)
        {
            _observer = observer;
            _name = name;

            _consumers = new ConsumerCollection<IGrpcQueueConsumer>();
            _consumer = TaskUtil.GetTask<IGrpcQueueConsumer>();
            _cancel = new CancellationTokenSource();

            var outputOptions = new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            };

            _channel = Channel.CreateUnbounded<DeliveryContext<GrpcTransportMessage>>(outputOptions);

            _dispatcher = Task.Run(() => StartDispatcher(_cancel.Token));
        }

        public ConnectHandle ConnectConsumer(NodeContext nodeContext, IGrpcQueueConsumer consumer)
        {
            try
            {
                var handle = _consumers.Connect(consumer);

                handle = _observer.ConsumerConnected(nodeContext, handle, _name);

                _consumer.TrySetResult(consumer);

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
                await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);
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

        void DeliverWithDelay(DeliveryContext<GrpcTransportMessage> context)
        {
            Task.Run(async () =>
            {
                var delay = context.Message.EnqueueTime.Value - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, context.CancellationToken).ConfigureAwait(false);

                await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);
            }, context.CancellationToken);
        }

        async Task StartDispatcher(CancellationToken cancellationToken)
        {
            try
            {

                // convert to convergent channels
                await _consumer.Task.OrCanceled(cancellationToken).ConfigureAwait(false);

                while (await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    DeliveryContext<GrpcTransportMessage> context = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        var consumer = _consumers.Next();
                        if (consumer != null)
                            await consumer.Consume(context.Message, context.CancellationToken).ConfigureAwait(false);
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
