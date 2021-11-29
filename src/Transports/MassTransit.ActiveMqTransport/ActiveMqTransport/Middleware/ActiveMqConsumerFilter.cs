namespace MassTransit.ActiveMqTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.NMS.ActiveMQ;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class ActiveMqConsumerFilter :
        IFilter<SessionContext>
    {
        readonly ActiveMqReceiveEndpointContext _context;

        public ActiveMqConsumerFilter(ActiveMqReceiveEndpointContext context)
        {
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<SessionContext>.Send(SessionContext context, IPipe<SessionContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var inputAddress = receiveSettings.GetInputAddress(context.ConnectionContext.HostAddress);

            var executor = new ChannelExecutor(receiveSettings.PrefetchCount, receiveSettings.ConcurrentMessageLimit);

            var consumers = new List<Task<ActiveMqConsumer>>
            {
                CreateConsumer(context, receiveSettings.EntityName, receiveSettings.Selector, receiveSettings.PrefetchCount, executor)
            };

            consumers.AddRange(_context.BrokerTopology.Consumers.Select(x =>
                CreateConsumer(context, x.Destination.EntityName, x.Selector, receiveSettings.PrefetchCount, executor)));

            ActiveMqConsumer[] actualConsumers = await Task.WhenAll(consumers).ConfigureAwait(false);

            var supervisor = CreateConsumerSupervisor(context, actualConsumers);

            LogContext.Debug?.Log("Consumers Ready: {InputAddress}", _context.InputAddress);

            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            try
            {
                await supervisor.Completed.ConfigureAwait(false);
            }
            finally
            {
                DeliveryMetrics[] consumerMetrics = actualConsumers.Cast<DeliveryMetrics>().ToArray();

                DeliveryMetrics metrics = new CombinedDeliveryMetrics(consumerMetrics.Sum(x => x.DeliveryCount),
                    consumerMetrics.Max(x => x.ConcurrentDeliveryCount));

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", inputAddress,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);

                await executor.DisposeAsync().ConfigureAwait(false);
            }
        }

        Supervisor CreateConsumerSupervisor(SessionContext context, ActiveMqConsumer[] actualConsumers)
        {
            var supervisor = new Supervisor();

            foreach (var consumer in actualConsumers)
                supervisor.Add(consumer);

            _context.AddConsumeAgent(supervisor);

            void HandleException(Exception exception)
            {
                supervisor.Stop(exception.Message);
            }

            context.ConnectionContext.Connection.ExceptionListener += HandleException;

            supervisor.SetReady();

            supervisor.Completed.ContinueWith(task => context.ConnectionContext.Connection.ExceptionListener -= HandleException,
                TaskContinuationOptions.ExecuteSynchronously);

            return supervisor;
        }

        async Task<ActiveMqConsumer> CreateConsumer(SessionContext context, string entityName, string selector, int prefetchCount, ChannelExecutor executor)
        {
            var queueName = $"{entityName}?consumer.prefetchSize={prefetchCount}";

            var queue = await context.GetQueue(queueName).ConfigureAwait(false);

            var messageConsumer = await context.CreateMessageConsumer(queue, selector, false).ConfigureAwait(false);

            LogContext.Debug?.Log("Created consumer for {InputAddress}: {Queue}", _context.InputAddress, queueName);

            var consumer = new ActiveMqConsumer(context, (MessageConsumer)messageConsumer, _context, executor);

            await consumer.Ready.ConfigureAwait(false);

            return consumer;
        }


        class CombinedDeliveryMetrics :
            DeliveryMetrics
        {
            public CombinedDeliveryMetrics(long deliveryCount, int concurrentDeliveryCount)
            {
                DeliveryCount = deliveryCount;
                ConcurrentDeliveryCount = concurrentDeliveryCount;
            }

            public long DeliveryCount { get; }
            public int ConcurrentDeliveryCount { get; }
        }
    }
}
