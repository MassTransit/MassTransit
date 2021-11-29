namespace MassTransit
{
    using System.Collections.Generic;
    using Amazon.SQS.Model;


    public interface AmazonSqsMessageContext
    {
        Message TransportMessage { get; }

        Dictionary<string, MessageAttributeValue> Attributes { get; }
    }
}
