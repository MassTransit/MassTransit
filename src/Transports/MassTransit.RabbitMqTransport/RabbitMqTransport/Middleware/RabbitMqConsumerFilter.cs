namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;
    using Topology;
    using Transports;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class RabbitMqConsumerFilter :
        IFilter<ModelContext>
    {
        readonly RabbitMqReceiveEndpointContext _context;
        string _consumerTag;

        public RabbitMqConsumerFilter(RabbitMqReceiveEndpointContext context)
        {
            _context = context;

            _consumerTag = "";
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var consumer = new RabbitMqBasicConsumer(context, _context);

            _consumerTag = await context.BasicConsume(receiveSettings.QueueName, receiveSettings.NoAck, _context.ExclusiveConsumer,
                receiveSettings.ConsumeArguments, consumer, _consumerTag).ConfigureAwait(false);

            await consumer.Ready.ConfigureAwait(false);

            _context.AddConsumeAgent(consumer);

            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                RabbitMqDeliveryMetrics metrics = consumer;
                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {ConsumerTag}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", metrics.ConsumerTag,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
