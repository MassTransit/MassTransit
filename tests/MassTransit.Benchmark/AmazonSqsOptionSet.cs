namespace MassTransitBenchmark
{
    using System;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using MassTransit;
    using MassTransit.AmazonSqsTransport;
    using MassTransit.Transports;
    using NDesk.Options;


    class AmazonSqsOptionSet :
        OptionSet,
        AmazonSqsHostSettings
    {
        string _accessKey;
        AWSCredentials _credentials;
        ImmutableCredentials _immutableCredentials;
        string _secretKey;

        public AmazonSqsOptionSet()
        {
            Add<string>("region:", "The AWS region", SetRegion);
            Add<string>("scope:", "Account Scope", value => Scope = value);
            Add<string>("accesskey:", "Access Key", SetAccessKey);
            Add<string>("secretkey:", "Secret Key", SetSecretKey);

            HostAddress = new Uri("amazonsqs://localhost:4566");

            Region = RegionEndpoint.APEast1;

            SetAccessKey("admin");
            SetSecretKey("admin");

            AmazonSqsConfig = new AmazonSQSConfig {ServiceURL = "http://localhost:4566"};
            AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig {ServiceURL = "http://localhost:4566"};
        }

        public AWSCredentials Credentials
        {
            get => _credentials;
            set
            {
                _credentials = value;
                _immutableCredentials = null;
            }
        }

        public AmazonSQSConfig AmazonSqsConfig { get; set; }
        public AmazonSimpleNotificationServiceConfig AmazonSnsConfig { get; set; }

        public string Scope { get; set; }

        public RegionEndpoint Region { get; set; }
        public string AccessKey => (_immutableCredentials ??= GetImmutableCredentials()).AccessKey;
        public string SecretKey => (_immutableCredentials ??= GetImmutableCredentials()).SecretKey;

        public AllowTransportHeader AllowTransportHeader { get; set; }

        public bool ScopeTopics => false;

        public Uri HostAddress { get; private set; }

        public IConnection CreateConnection()
        {
            return new Connection(Credentials, Region, AmazonSqsConfig, AmazonSnsConfig);
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = "https",
                Host = Region.SystemName
            }.Uri.ToString();
        }

        ImmutableCredentials GetImmutableCredentials()
        {
            return Credentials?.GetCredentials() ?? throw new ArgumentNullException(nameof(Credentials));
        }

        void SetRegion(string region)
        {
            HostAddress = new UriBuilder("amazonsqs", region)
            {
                Path = Scope
            }.Uri;

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            AmazonSqsConfig = new AmazonSQSConfig {RegionEndpoint = regionEndpoint};
            AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig {RegionEndpoint = regionEndpoint};
        }

        void SetAccessKey(string accessKey)
        {
            _accessKey = accessKey;
            SetBasicCredentials();
        }

        void SetSecretKey(string secretKey)
        {
            _secretKey = secretKey;
            SetBasicCredentials();
        }

        void SetBasicCredentials()
        {
            if (string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_secretKey))
                return;

            _credentials = new BasicAWSCredentials(_accessKey, _secretKey);
        }

        public void ShowOptions()
        {
            Console.WriteLine("Host: {0}", HostAddress);
        }
    }
}