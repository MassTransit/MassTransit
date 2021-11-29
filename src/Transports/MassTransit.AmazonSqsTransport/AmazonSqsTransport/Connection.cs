namespace MassTransit.AmazonSqsTransport
{
    using Amazon;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public class Connection :
        IConnection
    {
        public Connection(AWSCredentials credentials, RegionEndpoint regionEndpoint = null, AmazonSQSConfig amazonSqsConfig = null,
            AmazonSimpleNotificationServiceConfig amazonSnsConfig = null)
        {
            amazonSqsConfig ??= new AmazonSQSConfig { RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1 };
            amazonSnsConfig ??= new AmazonSimpleNotificationServiceConfig { RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1 };

            SqsClient = credentials == null
                ? new AmazonSQSClient(amazonSqsConfig)
                : new AmazonSQSClient(credentials, amazonSqsConfig);

            SnsClient = credentials == null
                ? new AmazonSimpleNotificationServiceClient(amazonSnsConfig)
                : new AmazonSimpleNotificationServiceClient(credentials, amazonSnsConfig);
        }

        public IAmazonSQS SqsClient { get; }
        public IAmazonSimpleNotificationService SnsClient { get; }

        public void Dispose()
        {
            SnsClient.Dispose();
            SqsClient.Dispose();
        }
    }
}
