namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ.Commands;
    using Context;
    using Transports;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext,
        TransportReceiveContext
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

        public IDictionary<string, object> GetTransportProperties()
        {
            var properties = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());

            if (TransportMessage.NMSPriority != MsgPriority.Normal)
                properties.Value[ActiveMqTransportPropertyNames.Priority] = TransportMessage.NMSPriority.ToString();
            if (GroupId != null)
                properties.Value[ActiveMqTransportPropertyNames.GroupId] = GroupId;
            if (GroupSequence != default)
                properties.Value[ActiveMqTransportPropertyNames.GroupSequence] = GroupSequence;

            return properties.IsValueCreated ? properties.Value : null;
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
