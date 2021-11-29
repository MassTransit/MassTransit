namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Transports;


    public class ConfigurationHostSettings :
        AmazonSqsHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        AWSCredentials _credentials;
        ImmutableCredentials _immutableCredentials;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
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

        public bool ScopeTopics { get; set; }

        public Uri HostAddress => _hostAddress.Value;

        public IConnection CreateConnection()
        {
            return new Connection(Credentials, Region, AmazonSqsConfig, AmazonSnsConfig);
        }

        Uri FormatHostAddress()
        {
            return new AmazonSqsHostAddress(Region.SystemName, Scope);
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
    }
}
