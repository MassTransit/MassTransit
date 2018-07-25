namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Amazon;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;

    public interface IConnection :
        IDisposable
    {
        string AccessKey { get; }

        string SecretKey { get; }

        AmazonSQSConfig AmazonSqsConfig { get; }

        AmazonSimpleNotificationServiceConfig AmazonSnsConfig { get; }

        IAmazonSQS CreateAmazonSqsClient();

        IAmazonSimpleNotificationService CreateAmazonSnsClient();
    }
    public class Connection :
        IConnection
    {
        public Connection(string accessKey, string secretKey, RegionEndpoint regionEndpoint = null, AmazonSQSConfig amazonSqsConfig = null, AmazonSimpleNotificationServiceConfig amazonSnsConfig = null)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
            AmazonSqsConfig = amazonSqsConfig ?? new AmazonSQSConfig { RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1 };
            AmazonSnsConfig = amazonSnsConfig ?? new AmazonSimpleNotificationServiceConfig { RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1 };
        }

        public string AccessKey { get; }

        public string SecretKey { get; }

        public AmazonSQSConfig AmazonSqsConfig { get; }

        public AmazonSimpleNotificationServiceConfig AmazonSnsConfig { get;  }

        public IAmazonSQS CreateAmazonSqsClient()
        {
            return new AmazonSQSClient(AccessKey, SecretKey, AmazonSqsConfig);
        }

        public IAmazonSimpleNotificationService CreateAmazonSnsClient()
        {
            return new AmazonSimpleNotificationServiceClient(AccessKey, SecretKey, AmazonSnsConfig);
        }

        public void Dispose()
        {
        }
    }
}