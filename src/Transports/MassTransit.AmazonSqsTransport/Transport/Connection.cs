namespace MassTransit.AmazonSqsTransport.Transport
{
    using Amazon;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public class Connection :
        IConnection
    {
        readonly AmazonSimpleNotificationServiceConfig _amazonSnsConfig;
        readonly AmazonSQSConfig _amazonSqsConfig;
        readonly AWSCredentials _credentials;

        public Connection(AWSCredentials credentials, RegionEndpoint regionEndpoint = null, AmazonSQSConfig amazonSqsConfig = null,
            AmazonSimpleNotificationServiceConfig amazonSnsConfig = null)
        {
            _credentials = credentials;
            _amazonSqsConfig = amazonSqsConfig ?? new AmazonSQSConfig {RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1};
            _amazonSnsConfig = amazonSnsConfig ?? new AmazonSimpleNotificationServiceConfig {RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1};
        }

        public IAmazonSQS CreateAmazonSqsClient()
        {
            return new AmazonSQSClient(_credentials, _amazonSqsConfig);
        }

        public IAmazonSimpleNotificationService CreateAmazonSnsClient()
        {
            return new AmazonSimpleNotificationServiceClient(_credentials, _amazonSnsConfig);
        }
    }
}
