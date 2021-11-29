namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System.Threading.Tasks;
    using Topology;
    using Transports;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class AmazonSqsConsumerFilter :
        IFilter<ClientContext>
    {
        readonly SqsReceiveEndpointContext _context;

        public AmazonSqsConsumerFilter(SqsReceiveEndpointContext context)
        {
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var inputAddress = receiveSettings.GetInputAddress(context.ConnectionContext.HostAddress);

            var receiver = new AmazonSqsMessageReceiver(context, _context);

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
        }
    }
}
