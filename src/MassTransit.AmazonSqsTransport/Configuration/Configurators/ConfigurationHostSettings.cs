namespace MassTransit.AmazonSqsTransport.Configuration.Configurators
{
    using System;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Transport;
    using Transports;


    public class ConfigurationHostSettings :
        AmazonSqsHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        ImmutableCredentials _immutableCredentials;
        AWSCredentials _credentials;

        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
        }

        public RegionEndpoint Region { get; set; }
        public string AccessKey => (_immutableCredentials ??= GetImmutableCredentials()).AccessKey;
        public string SecretKey => (_immutableCredentials ??= GetImmutableCredentials()).SecretKey;

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
        public string VirtualHost { get; set; }

        public AllowTransportHeader AllowTransportHeader { get; set; }

        public Uri HostAddress => _hostAddress.Value;

        public IConnection CreateConnection()
        {
            return new Connection(Credentials, Region, AmazonSqsConfig, AmazonSnsConfig);
        }

        Uri FormatHostAddress()
        {
            return new AmazonSqsHostAddress(Region.SystemName, Scope, VirtualHost);
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
