namespace MassTransitBenchmark
{
    using System;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Internals;
    using MassTransit.RabbitMqTransport;
    using MassTransit.RabbitMqTransport.Configuration;
    using MassTransit.Transports;
    using NDesk.Options;
    using RabbitMQ.Client;


    class RabbitMqOptionSet :
        OptionSet,
        RabbitMqHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        BatchSettings _batchSettings;

        public RabbitMqOptionSet()
        {
            Add<string>("h|host:", "The host name of the broker", x => Host = x);
            Add<string>("vhost:", "The virtual host to use", value => VirtualHost = value);
            Add<string>("u|username:", "Username (if using basic credentials)", value => Username = value);
            Add<string>("p|password:", "Password (if using basic credentials)", value => Password = value);
            Add<TimeSpan>("heartbeat:", "Heartbeat (for RabbitMQ)", value => Heartbeat = value);
            Add<bool>("confirm:", "Publisher Confirmation", value => PublisherConfirmation = value);
            Add<bool>("batch:", "Batch Publish", EnableBatch);
            Add<int>("batch-limit:", "Batch message limit", value => BatchLimit = value);
            Add<int>("batch-timeout:", "Batch Publish", value => BatchTimeout = value);
            Add<bool>("ssl:", "Use SSL", EnableSsl);
            Add<bool>("split:", "Split into two bus instances to leverage separate connections", x => Split = x);

            Host = "localhost";
            Username = "guest";
            Password = "guest";
            Heartbeat = TimeSpan.Zero;
            VirtualHost = "/";
            Port = 5672;

            Ssl = false;
            SslProtocol = SslProtocols.Tls12;
            SslServerName = Host;
            AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch;
            ClientCertificatePath = "";
            ClientCertificatePassphrase = "";

            RequestedConnectionTimeout = TimeSpan.FromSeconds(10);
            MessageNameFormatter = new RabbitMqMessageNameFormatter();

            PublisherConfirmation = false;
            EnableBatch(true);

            _hostAddress = new Lazy<Uri>(FormatHostAddress);
        }

        public IMessageNameFormatter MessageNameFormatter { get; }

        public string[] ClusterMembers => null;
        public bool Split { get; set; }

        public int BatchLimit { get; set; } = 100;
        public int BatchTimeout { get; set; } = 1;

        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan Heartbeat { get; set; }

        public bool Ssl { get; private set; }

        public SslProtocols SslProtocol { get; }
        public string SslServerName { get; }
        public SslPolicyErrors AcceptablePolicyErrors { get; }
        public string ClientCertificatePath { get; }
        public string ClientCertificatePassphrase { get; }

        public X509Certificate ClientCertificate => null;
        public bool UseClientCertificateAsAuthenticationIdentity => false;

        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }

        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
        public IRabbitMqEndpointResolver EndpointResolver => null;

        public string ClientProvidedName => "mtbench";

        public Uri HostAddress => _hostAddress.Value;

        public bool PublisherConfirmation { get; set; }

        public ushort RequestedChannelMax { get; }

        public TimeSpan RequestedConnectionTimeout { get; }

        public BatchSettings BatchSettings => _batchSettings;

        public TimeSpan ContinuationTimeout => TimeSpan.FromSeconds(20);

        public Task Refresh(ConnectionFactory connectionFactory)
        {
            return Task.CompletedTask;
        }

        Uri FormatHostAddress()
        {
            var builder = new UriBuilder
            {
                Scheme = "rabbitmq",
                Host = Host,
                Port = Port == 5672 ? 0 : Port,
                Path = string.IsNullOrWhiteSpace(VirtualHost) || VirtualHost == "/"
                    ? "/"
                    : $"/{VirtualHost.Trim('/')}"
            };

            return builder.Uri;
        }

        void EnableBatch(bool enabled)
        {
            _batchSettings = new ConfigurationBatchSettings
            {
                Enabled = enabled,
                MessageLimit = BatchLimit,
                SizeLimit = 200000,
                Timeout = TimeSpan.FromMilliseconds(BatchTimeout)
            };
        }

        void EnableSsl(bool enabled)
        {
            Ssl = true;
            Port = 5671;
        }

        public void ShowOptions()
        {
            Console.WriteLine("Host: {0}", Host);
            Console.WriteLine("Virtual Host: {0}", VirtualHost);
            Console.WriteLine("Username: {0}", Username);
            Console.WriteLine("Password: {0}", new string('*', (Password ?? "default").Length));
            Console.WriteLine("Heartbeat: {0}", Heartbeat);
            Console.WriteLine("Publisher Confirmation: {0}", PublisherConfirmation);
            Console.WriteLine("Split: {0}", Split);
            Console.WriteLine("Batch: enabled={0}, limit={1}, timeout={2}", _batchSettings.Enabled, _batchSettings.MessageLimit,
                _batchSettings.Timeout.ToFriendlyString());
        }
    }


    class ConfigurationBatchSettings :
        BatchSettings
    {
        public bool Enabled { get; set; }

        public int MessageLimit { get; set; }

        public int SizeLimit { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
