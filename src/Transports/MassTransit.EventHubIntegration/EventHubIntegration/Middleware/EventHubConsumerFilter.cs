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
            var receiveSettings = _context.GetPayload<ReceiveSettings>();

            var receiver = new EventHubDataReceiver(receiveSettings, _context, context);

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

                _context.LogConsumerCompleted(metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
