namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using Amazon.SQS.Model;
    using Transports;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext
    {
        public AmazonSqsReceiveContext(Message message, bool redelivered, SqsReceiveEndpointContext context, ClientContext clientContext,
            ReceiveSettings settings, ConnectionContext connectionContext)
            : base(redelivered, context, settings, clientContext, connectionContext)
        {
            TransportMessage = message;

            Body = new StringMessageBody(message?.Body);
        }

        protected override IHeaderProvider HeaderProvider => new AmazonSqsHeaderProvider(TransportMessage);

        public override MessageBody Body { get; }

        public Message TransportMessage { get; }

        public Dictionary<string, MessageAttributeValue> Attributes => TransportMessage.MessageAttributes;
    }
}
