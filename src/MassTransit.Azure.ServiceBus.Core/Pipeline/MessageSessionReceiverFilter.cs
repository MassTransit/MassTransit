namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Transport;


    /// <summary>
    /// Creates a message session receiver
    /// </summary>
    public class MessageSessionReceiverFilter :
        MessageReceiverFilter
    {
        public MessageSessionReceiverFilter(IBrokeredMessageReceiver messageReceiver, IReceiveTransportObserver transportObserver)
            : base(messageReceiver, transportObserver)
        {
        }

        protected override IReceiver CreateMessageReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            return new SessionReceiver(context, messageReceiver);
        }
    }
}
