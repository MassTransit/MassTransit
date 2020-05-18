namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Transports.Metrics;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class ActiveMqConsumerFilter :
        Supervisor,
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

            List<Task<ActiveMqBasicConsumer>> consumers = new List<Task<ActiveMqBasicConsumer>>
            {
                CreateConsumer(context, receiveSettings.EntityName, receiveSettings.Selector, receiveSettings.PrefetchCount)
            };

            consumers.AddRange(_context.BrokerTopology.Consumers.Select(x =>
                CreateConsumer(context, x.Destination.EntityName, x.Selector, receiveSettings.PrefetchCount)));

            var actualConsumers = await Task.WhenAll(consumers).ConfigureAwait(false);

            var supervisor = CreateConsumerSupervisor(context, actualConsumers);

            await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await supervisor.Completed.ConfigureAwait(false);
            }
            finally
            {
                var consumerMetrics = actualConsumers.Cast<DeliveryMetrics>().ToArray();

                DeliveryMetrics metrics = new CombinedDeliveryMetrics(consumerMetrics.Sum(x => x.DeliveryCount),
                    consumerMetrics.Max(x => x.ConcurrentDeliveryCount));

                await _context.TransportObservers.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", inputAddress,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }

        Supervisor CreateConsumerSupervisor(SessionContext context, ActiveMqBasicConsumer[] actualConsumers)
        {
            Supervisor supervisor = new Supervisor();

            foreach (var consumer in actualConsumers)
            {
                supervisor.Add(consumer);
            }

            Add(supervisor);

            void HandleException(Exception exception)
            {
                supervisor.Stop(exception.Message);
            }

            context.ConnectionContext.Connection.ExceptionListener += HandleException;

            supervisor.SetReady();

            supervisor.Completed.ContinueWith(task => context.ConnectionContext.Connection.ExceptionListener -= HandleException);

            return supervisor;
        }

        async Task<ActiveMqBasicConsumer> CreateConsumer(SessionContext context, string entityName, string selector, ushort prefetchCount)
        {
            string queueName = $"{entityName}?consumer.prefetchSize={prefetchCount}";

            var queue = await context.GetQueue(queueName).ConfigureAwait(false);

            var messageConsumer = await context.CreateMessageConsumer(queue, selector, false).ConfigureAwait(false);

            LogContext.Debug?.Log("Created consumer for {InputAddress}: {Queue}", _context.InputAddress, queueName);

            var consumer = new ActiveMqBasicConsumer(context, messageConsumer, _context);

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
