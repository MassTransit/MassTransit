#nullable enable
namespace MassTransit.InMemoryTransport
{
    using Transports;


    public sealed class InMemoryReceiveContext :
        BaseReceiveContext,
        RoutingKeyConsumeContext
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
        public string? RoutingKey => _message.RoutingKey;
    }
}
