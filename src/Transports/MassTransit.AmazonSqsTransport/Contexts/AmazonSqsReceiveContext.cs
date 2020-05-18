namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Context;
    using Exceptions;
    using Metadata;
    using Topology;
    using Transports;
    using Util;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext,
        ReceiveLockContext
    {
        readonly ClientContext _clientContext;
        readonly ReceiveSettings _receiveSettings;
        byte[] _body;

        public AmazonSqsReceiveContext(Message transportMessage, bool redelivered, SqsReceiveEndpointContext context,
            ClientContext clientContext, ReceiveSettings receiveSettings, ConnectionContext connectionContext)
            : base(redelivered, context, receiveSettings, clientContext, connectionContext)
        {
            _clientContext = clientContext;
            _receiveSettings = receiveSettings;
            TransportMessage = transportMessage;
        }

        protected override IHeaderProvider HeaderProvider => new AmazonSqsHeaderProvider(TransportMessage);

        public Message TransportMessage { get; }

        public Dictionary<string, MessageAttributeValue> Attributes => TransportMessage.MessageAttributes;

        public override byte[] GetBody()
        {
            if (_body != null)
                return _body;

            if (TransportMessage != null)
            {
                return _body = Encoding.UTF8.GetBytes(TransportMessage.Body);
            }

            throw new AmazonSqsTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(typeof(Message))}");
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody());
        }

        public Task Complete()
        {
            return _clientContext.DeleteMessage(_receiveSettings.EntityName, TransportMessage.ReceiptHandle);
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
