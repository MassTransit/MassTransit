namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Context;
    using Metadata;
    using Transports;
    using Util;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext,
        ReceiveLockContext
    {
        byte[] _body;

        public ActiveMqReceiveContext(IMessage transportMessage, ActiveMqReceiveEndpointContext context, params object[] payloads)
            : base(transportMessage.NMSRedelivered, context, payloads)
        {
            TransportMessage = transportMessage;
        }

        protected override IHeaderProvider HeaderProvider => new ActiveMqHeaderProvider(TransportMessage);

        public IMessage TransportMessage { get; }

        public IPrimitiveMap Properties => TransportMessage.Properties;

        public Task Complete()
        {
            TransportMessage.Acknowledge();

            return TaskUtil.Completed;
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task ValidateLockStatus()
        {
            return TaskUtil.Completed;
        }

        public override byte[] GetBody()
        {
            if (_body != null)
                return _body;

            if (TransportMessage is ITextMessage textMessage)
                return _body = Encoding.UTF8.GetBytes(textMessage.Text);

            if (TransportMessage is IBytesMessage bytesMessage)
                return _body = bytesMessage.Content;

            throw new ActiveMqTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(TransportMessage.GetType())}");
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody());
        }
    }
}
