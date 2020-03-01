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
        readonly IMessage _transportMessage;
        byte[] _body;

        public ActiveMqReceiveContext(IMessage transportMessage, ActiveMqReceiveEndpointContext context, params object[] payloads)
            : base(transportMessage.NMSRedelivered, context, payloads)
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

        public Task Complete()
        {
            _transportMessage.Acknowledge();

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
    }
}
