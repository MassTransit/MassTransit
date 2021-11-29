namespace MassTransit.InMemoryTransport
{
    using Fabric;
    using Transports;


    public sealed class InMemoryReceiveContext :
        BaseReceiveContext
    {
        readonly InMemoryTransportMessage _message;

        public InMemoryReceiveContext(InMemoryTransportMessage message, InMemoryReceiveEndpointContext receiveEndpointContext)
            : base(message.DeliveryCount > 0, receiveEndpointContext)
        {
            _message = message;

            Body = new BytesMessageBody(message.Body);
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_message.Headers);

        public override MessageBody Body { get; }
    }
}
