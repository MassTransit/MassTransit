namespace MassTransit.EventHubIntegration.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    public class EventHubConsumerFilter :
        IFilter<ProcessorContext>
    {
        readonly ReceiveEndpointContext _context;

        public EventHubConsumerFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(ProcessorContext context, IPipe<ProcessorContext> next)
        {
            var inputAddress = _context.InputAddress;

            IEventHubDataReceiver receiver = new EventHubDataReceiver(_context, context);

            await receiver.Start().ConfigureAwait(false);

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
