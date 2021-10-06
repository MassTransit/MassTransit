namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public interface IConnection :
        IDisposable
    {
        IAmazonSQS SqsClient { get; }

        IAmazonSimpleNotificationService SnsClient { get; }
    }
}
