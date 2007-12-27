namespace MassTransit.ServiceBus
{
    public class MessageQueueEndpoint :
        IEndpoint
    {
        private readonly ITransport _transport;

        public MessageQueueEndpoint(ITransport transport)
        {
            _transport = transport;
        }

        public ITransport Transport
        {
            get { return _transport; }
        }


        public static implicit operator MessageQueueEndpoint(string queuePath)
        {
            ITransport transport = new MsmqTransport(queuePath);

            return new MessageQueueEndpoint(transport);
        }

        public static implicit operator string(MessageQueueEndpoint endpoint)
        {
            return endpoint.Transport.Address;
        }
    }
}