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

        RegionEndpoint Region { get; }

        IAmazonSQS CreateAmazonSqs();

        IAmazonSimpleNotificationService CreateAmazonSns();
    }
    public class Connection :
        IConnection
    {
        public Connection(string accessKey, string secretKey, RegionEndpoint region, string serviceUrl = null)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
            Region = region;
            ServiceUrl = serviceUrl;
        }

        public string AccessKey { get; }

        public string SecretKey { get; }

        public RegionEndpoint Region { get; }

        public string ServiceUrl { get; }

        public IAmazonSQS CreateAmazonSqs()
        {
            var config = new AmazonSQSConfig
            {
                RegionEndpoint = Region ?? RegionEndpoint.USEast1
            };

            if (ServiceUrl != null)
            {
                config.ServiceURL = ServiceUrl;
            }

            return new AmazonSQSClient(AccessKey, SecretKey, config);
        }

        public IAmazonSimpleNotificationService CreateAmazonSns()
        {
            var config = new AmazonSimpleNotificationServiceConfig
            {
                RegionEndpoint = Region ?? RegionEndpoint.USEast1
            };

            if (ServiceUrl != null)
            {
                config.ServiceURL = ServiceUrl;
            }

            return new AmazonSimpleNotificationServiceClient(AccessKey, SecretKey, config);
        }

        public void Dispose()
        {
        }
    }
}