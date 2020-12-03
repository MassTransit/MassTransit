namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using Transport;


    /// <summary>
    /// Creates a message session receiver
    /// </summary>
    public class MessageSessionReceiverFilter :
        MessageReceiverFilter
    {
        public MessageSessionReceiverFilter(IBrokeredMessageReceiver messageReceiver, ServiceBusReceiveEndpointContext context)
            : base(messageReceiver, context)
        {
        }

        protected override IReceiver CreateMessageReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            return new SessionReceiver(context, messageReceiver);
        }
    }
}
