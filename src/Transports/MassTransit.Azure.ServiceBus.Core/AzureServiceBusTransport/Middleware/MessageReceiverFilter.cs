namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// Creates a message receiver and receives messages from the input queue of the endpoint
    /// </summary>
    public class MessageReceiverFilter :
        IFilter<ClientContext>
    {
        readonly ServiceBusReceiveEndpointContext _context;
        readonly IServiceBusMessageReceiver _messageReceiver;
        readonly IReceiveTransportObserver _transportObserver;

        public MessageReceiverFilter(IServiceBusMessageReceiver messageReceiver, ServiceBusReceiveEndpointContext context)
        {
            _messageReceiver = messageReceiver;
            _transportObserver = context.TransportObservers;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("messageReceiver");
            _messageReceiver.Probe(scope);
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            if (context.IsClosedOrClosing)
                return;

            var receiver = CreateMessageReceiver(context, _messageReceiver);

            await receiver.Start().ConfigureAwait(false);

            await receiver.Ready.ConfigureAwait(false);

            _context.AddConsumeAgent(receiver);

            await _transportObserver.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            try
            {
                await receiver.Completed.ConfigureAwait(false);
            }
            finally
            {
                var metrics = receiver.GetDeliveryMetrics();

                await _transportObserver.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", context.InputAddress,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        protected virtual IReceiver CreateMessageReceiver(ClientContext context, IServiceBusMessageReceiver messageReceiver)
        {
            return new Receiver(context, messageReceiver);
        }
    }
}
