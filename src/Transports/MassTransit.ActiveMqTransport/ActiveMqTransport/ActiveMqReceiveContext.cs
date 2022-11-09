namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using Apache.NMS.ActiveMQ.Commands;
    using System;
    using System.Threading.Tasks;
    using Transports;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext,
        ReceiveLockContext
    {
        public ActiveMqReceiveContext(IMessage transportMessage, ActiveMqReceiveEndpointContext context, params object[] payloads)
            : base(transportMessage.NMSRedelivered, context, payloads)
        {
            TransportMessage = transportMessage;

            Body = new ActiveMqMessageBody(transportMessage);
        }

        protected override IHeaderProvider HeaderProvider => new ActiveMqHeaderProvider(TransportMessage);

        public override MessageBody Body { get; }

        public IMessage TransportMessage { get; }

        public IPrimitiveMap Properties => TransportMessage.Properties;

        public string GroupId => TransportMessage is Message message ? message.GroupID : null;

        public int GroupSequence => TransportMessage is Message message ? message.GroupSequence : default;

        public Task Complete()
        {
            TransportMessage.Acknowledge();

            return Task.CompletedTask;
        }

        public Task Faulted(Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            var provider = base.GetSendEndpointProvider();

            return TransportMessage.NMSReplyTo != null
                ? new ReceiveSendEndpointProvider(provider, TransportMessage.NMSReplyTo)
                : provider;
        }

        class ReceiveSendEndpointProvider :
            ISendEndpointProvider
        {
            readonly IDestination _replyTo;
            readonly ISendEndpointProvider _sendEndpointProvider;

            public ReceiveSendEndpointProvider(ISendEndpointProvider sendEndpointProvider, IDestination replyTo)
            {
                _replyTo = replyTo;

                _sendEndpointProvider = sendEndpointProvider;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _sendEndpointProvider.ConnectSendObserver(observer);
            }

            public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

                return new ReplyToSendEndpoint(endpoint, _replyTo);
            }
        }
    }
}
