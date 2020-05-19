namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class RabbitMqConsumerFilter :
        Supervisor,
        IFilter<ModelContext>
    {
        readonly RabbitMqReceiveEndpointContext _context;

        public RabbitMqConsumerFilter(RabbitMqReceiveEndpointContext context)
        {
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var consumer = new RabbitMqBasicConsumer(context, _context);

            await context.BasicConsume(receiveSettings.QueueName, receiveSettings.NoAck, _context.ExclusiveConsumer, receiveSettings
            .ConsumeArguments, consumer)
                .ConfigureAwait(false);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(_context.InputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                RabbitMqDeliveryMetrics metrics = consumer;
                await _context.TransportObservers.Completed(new ReceiveTransportCompletedEvent(_context.InputAddress, metrics)).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {ConsumerTag}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", metrics.ConsumerTag,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }
    }
}
