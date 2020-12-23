namespace MassTransit.EventHubIntegration.Filters
{
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using Transports.Metrics;


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
