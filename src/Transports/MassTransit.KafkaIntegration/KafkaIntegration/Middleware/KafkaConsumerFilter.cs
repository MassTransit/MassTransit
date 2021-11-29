namespace MassTransit.KafkaIntegration.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    public class KafkaConsumerFilter<TKey, TValue> :
        IFilter<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly ReceiveEndpointContext _context;

        public KafkaConsumerFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(ConsumerContext<TKey, TValue> context, IPipe<ConsumerContext<TKey, TValue>> next)
        {
            var inputAddress = _context.InputAddress;

            IKafkaMessageReceiver<TKey, TValue> receiver = new KafkaMessageReceiver<TKey, TValue>(_context, context);

            await receiver.Ready.ConfigureAwait(false);

            _context.AddConsumeAgent(receiver);

            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            try
            {
                await receiver.Completed.ConfigureAwait(false);
            }
            finally
            {
                DeliveryMetrics metrics = receiver;

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", inputAddress,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
