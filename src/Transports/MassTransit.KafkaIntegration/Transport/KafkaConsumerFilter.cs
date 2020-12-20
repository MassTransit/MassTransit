namespace MassTransit.KafkaIntegration.Transport
{
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using Transports.Metrics;


    public class KafkaConsumerFilter<TKey, TValue> :
        IFilter<IKafkaConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly ReceiveEndpointContext _context;

        public KafkaConsumerFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(IKafkaConsumerContext<TKey, TValue> context, IPipe<IKafkaConsumerContext<TKey, TValue>> next)
        {
            var inputAddress = _context.InputAddress;

            var receiver = new KafkaMessageReceiver<TKey, TValue>(_context, context);

            await receiver.Ready.ConfigureAwait(false);

            _context.AddAgent(receiver);

            LogContext.Debug?.Log("Receiver Ready: {InputAddress}", _context.InputAddress);

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
