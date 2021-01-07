namespace MassTransit.KafkaIntegration.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using Transport;
    using Transports.Metrics;


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

            await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await receiver.Completed.ConfigureAwait(false);
            }
            finally
            {
                DeliveryMetrics metrics = receiver;

                await _context.TransportObservers.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics));

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
