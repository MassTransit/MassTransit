namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Transport;


    /// <summary>
    /// Creates a message receiver and receives messages from the input queue of the endpoint
    /// </summary>
    public class MessageReceiverFilter :
        Supervisor,
        IFilter<ClientContext>
    {
        readonly IBrokeredMessageReceiver _messageReceiver;
        readonly IReceiveTransportObserver _transportObserver;

        public MessageReceiverFilter(IBrokeredMessageReceiver messageReceiver, IReceiveTransportObserver transportObserver)
        {
            _messageReceiver = messageReceiver;
            _transportObserver = transportObserver;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("messageReceiver");
            _messageReceiver.Probe(scope);
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            LogContext.Debug?.Log("Creating message receiver for {InputAddress}", context.InputAddress);

            var receiver = CreateMessageReceiver(context, _messageReceiver);

            await receiver.Start().ConfigureAwait(false);

            await receiver.Ready.ConfigureAwait(false);

            Add(receiver);

            await _transportObserver.Ready(new ReceiveTransportReadyEvent(context.InputAddress)).ConfigureAwait(false);

            try
            {
                await receiver.Completed.ConfigureAwait(false);
            }
            finally
            {
                var metrics = receiver.GetDeliveryMetrics();

                await _transportObserver.Completed(new ReceiveTransportCompletedEvent(context.InputAddress, metrics)).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", context.InputAddress,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        protected virtual IReceiver CreateMessageReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            return new Receiver(context, messageReceiver);
        }
    }
}
