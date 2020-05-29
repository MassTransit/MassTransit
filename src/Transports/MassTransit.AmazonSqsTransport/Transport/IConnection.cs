namespace MassTransit.AmazonSqsTransport.Transport
{
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public interface IConnection
    {
        IAmazonSQS CreateAmazonSqsClient();

        IAmazonSimpleNotificationService CreateAmazonSnsClient();
    }
}
