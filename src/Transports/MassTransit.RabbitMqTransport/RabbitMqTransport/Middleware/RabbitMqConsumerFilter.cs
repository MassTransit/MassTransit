namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// A filter that uses the channel context to create a basic consumer and connect it to the channel
    /// </summary>
    public class RabbitMqConsumerFilter :
        IFilter<ChannelContext>
    {
        readonly RabbitMqReceiveEndpointContext _context;
        string _consumerTag;

        public RabbitMqConsumerFilter(RabbitMqReceiveEndpointContext context)
        {
            _context = context;

            _consumerTag = "";
        }

        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(ChannelContext context, IPipe<ChannelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            if (string.IsNullOrWhiteSpace(_consumerTag) && !string.IsNullOrWhiteSpace(receiveSettings.ConsumerTag))
                _consumerTag = receiveSettings.ConsumerTag;

            var consumer = new RabbitMqBasicConsumer(context, _context);

            _consumerTag = await context.BasicConsume(receiveSettings.QueueName, receiveSettings.NoAck, _context.ExclusiveConsumer,
                receiveSettings.ConsumeArguments, consumer, _consumerTag, context.CancellationToken).ConfigureAwait(false);

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

                _context.LogConsumerCompleted(metrics.DeliveryCount, metrics.ConcurrentDeliveryCount, metrics.ConsumerTag);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
