namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Text;
    using Apache.NMS;
    using Context;
    using Metadata;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext
    {
        readonly IMessage _transportMessage;
        byte[] _body;

        public ActiveMqReceiveContext(Uri inputAddress, IMessage transportMessage, ActiveMqReceiveEndpointContext context, params object[] payloads)
            : base(inputAddress, transportMessage.NMSRedelivered, context, payloads)
        {
            _transportMessage = transportMessage;
        }

        protected override IHeaderProvider HeaderProvider => new ActiveMqHeaderProvider(_transportMessage);

        public IMessage TransportMessage => _transportMessage;

        public IPrimitiveMap Properties => _transportMessage.Properties;

        public override byte[] GetBody()
        {
            if (_body != null)
                return _body;

            if (_transportMessage is ITextMessage textMessage)
                return _body = Encoding.UTF8.GetBytes(textMessage.Text);

            if (_transportMessage is IBytesMessage bytesMessage)
                return _body = bytesMessage.Content;

            throw new ActiveMqTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(_transportMessage.GetType())}");
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody());
        }
    }
}
