namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Amazon.SQS.Model;
    using Context;
    using Exceptions;
    using Metadata;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext
    {
        byte[] _body;

        public AmazonSqsReceiveContext(Uri inputAddress, Message transportMessage, bool redelivered, SqsReceiveEndpointContext context,
            params object[] payloads)
            : base(inputAddress, redelivered, context, payloads)
        {
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
    }
}
