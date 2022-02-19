#nullable enable
namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Middleware;


    public class MessageQueue<TContext, T> :
        Agent,
        IMessageQueue<TContext, T>
        where T : class
        where TContext : class
    {
        readonly Channel<DeliveryContext<T>> _channel;
        readonly Task _dispatcher;
        readonly QueueMetric _metrics;
        readonly IMessageFabricObserver<TContext> _observer;
        readonly MessageReceiverCollection<T> _receivers;

        public MessageQueue(IMessageFabricObserver<TContext> observer, string name)
        {
            _observer = observer;
            Name = name;

            _receivers = new MessageReceiverCollection<T>(receivers => new RoundRobinReceiverLoadBalancer<T>(receivers));
            _metrics = new QueueMetric(name);

            _channel = Channel.CreateUnbounded<DeliveryContext<T>>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            });

            _dispatcher = Task.Run(() => StartDispatcher());
        }

        public string Name { get; }

        public TopologyHandle ConnectMessageReceiver(TContext nodeContext, IMessageReceiver<T> receiver)
        {
            try
            {
                var handle = _receivers.Connect(receiver);

                handle = _observer.ConsumerConnected(nodeContext, handle, Name);

                return handle;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException($"Only a single consumer can be connected to a queue: {Name}", exception);
            }
        }

        public async Task Deliver(DeliveryContext<T> context)
        {
            if (context.WasAlreadyDelivered(this))
                return;

            if (context.EnqueueTime.HasValue)
                DeliverWithDelay(context);
            else
            {
                await _channel.Writer.WriteAsync(context, context.CancellationToken).ConfigureAwait(false);

                _metrics.MessageCount.Add();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("queue");
            scope.Add("name", Name);

            _receivers.Probe(scope);
        }

        protected override async Task StopAgent(StopContext context)
        {
            _channel.Writer.Complete();

            await _channel.Reader.Completion.ConfigureAwait(false);

            await _dispatcher.ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);
        }

        void DeliverWithDelay(DeliveryContext<T> context)
        {
            Task.Run(async () =>
            {
                var delayed = false;
                try
                {
                    var delay = context.EnqueueTime!.Value - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        _metrics.DelayedMessageCount.Add();
                        delayed = true;

                        await Task.Delay(delay, Stopping).ConfigureAwait(false);
                    }

                    await _channel.Writer.WriteAsync(context, Stopping).ConfigureAwait(false);

                    _metrics.MessageCount.Add();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "Message delivery faulted: {Queue}", Name);
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
                while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    if (!_channel.Reader.TryRead(out DeliveryContext<T>? context))
                        continue;

                    _metrics.MessageCount.Remove();

                    try
                    {
                        IMessageReceiver<T>? receiver = null;

                        if (context.ReceiverId.HasValue)
                        {
                            if (!_receivers.TryGetReceiver(context.ReceiverId.Value, out receiver))
                                LogContext.Debug?.Log("Receiver not found: {Queue}, {ReceiverId}", Name, context.ReceiverId.Value);
                        }

                        receiver ??= await _receivers.Next(context.Message, Stopping).ConfigureAwait(false);
                        if (receiver != null)
                            await receiver.Deliver(context.Message, Stopping).ConfigureAwait(false);
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
                LogContext.Warning?.Log(exception, "Queue dispatcher faulted: {Queue}", Name);
            }
        }
    }
}
