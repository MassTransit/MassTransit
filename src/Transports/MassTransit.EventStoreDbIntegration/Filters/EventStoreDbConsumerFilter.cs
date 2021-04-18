using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.Events;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.Transports.Metrics;

namespace MassTransit.EventStoreDbIntegration.Filters
{
    public sealed class EventStoreDbConsumerFilter :
        IFilter<ProcessorContext>
    {
        readonly ReceiveEndpointContext _context;

        public EventStoreDbConsumerFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(ProcessorContext context, IPipe<ProcessorContext> next)
        {
            var inputAddress = _context.InputAddress;

            IEventStoreDbDataReceiver receiver = new EventStoreDbDataReceiver(_context, context);

            await receiver.Start().ConfigureAwait(false);

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
