namespace MassTransit.AmazonSqsTransport.Testing
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Configurators;
    using MassTransit.Testing;


    public class AmazonSqsTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public AmazonSqsTestHarness(string host, string accessKey, string secretKey)
        {
            HostAddress = new Uri("amazonsqs://" + host);

            AccessKey = accessKey;
            SecretKey = secretKey;

            InputQueueName = "input_queue";
        }

        public AmazonSqsTestHarness()
        {
            AccessKey = "admin";
            SecretKey = "admin";

            AmazonSqsConfig = new AmazonSQSConfig {ServiceURL = "http://localhost:4576"};
            AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig {ServiceURL = "http://localhost:4575"};

            InputQueueName = "input_queue";

            HostAddress = new Uri("amazonsqs://localhost:4576");
        }

        public Uri HostAddress
        {
            get => _hostAddress;
            set
            {
                _hostAddress = value;
                _inputQueueAddress = new Uri(HostAddress, InputQueueName);
            }
        }

        public string AccessKey { get; }
        public string SecretKey { get; }
        public AmazonSQSConfig AmazonSqsConfig { get; private set; }
        public AmazonSimpleNotificationServiceConfig AmazonSnsConfig { get; private set; }
        public override string InputQueueName { get; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IAmazonSqsBusFactoryConfigurator> OnConfigureAmazonSqsBus;
        public event Action<IAmazonSqsReceiveEndpointConfigurator> OnConfigureAmazonSqsReceiveEndpoint;
        public event Action<IAmazonSqsHostConfigurator> OnConfigureAmazonSqsHost;
        public event Action<IAmazonSQS, IAmazonSimpleNotificationService> OnCleanupVirtualHost;

        protected virtual void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            OnConfigureAmazonSqsBus?.Invoke(configurator);
        }

        protected virtual void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            OnConfigureAmazonSqsReceiveEndpoint?.Invoke(configurator);
        }

        protected virtual void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            OnConfigureAmazonSqsHost?.Invoke(configurator);
        }

        protected virtual void CleanupVirtualHost(IAmazonSQS amazonSqs, IAmazonSimpleNotificationService amazonSns)
        {
            OnCleanupVirtualHost?.Invoke(amazonSqs, amazonSns);
        }

        protected virtual void ConfigureHost(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.Host(HostAddress, h =>
            {
                ConfigureHostSettings(h);
            });
        }

        public AmazonSqsHostSettings GetHostSettings()
        {
            var host = new AmazonSqsHostConfigurator(HostAddress);

            ConfigureHostSettings(host);

            return host.Settings;
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAmazonSqs(x =>
            {
                ConfigureHost(x);

                // CleanUpVirtualHost();

                ConfigureBus(x);

                ConfigureAmazonSqsBus(x);

                x.ReceiveEndpoint(InputQueueName, e =>
                {
                    e.PrefetchCount = 1;
                    e.WaitTimeSeconds = 0;
                    e.PurgeOnStartup = true;

                    ConfigureReceiveEndpoint(e);

                    ConfigureAmazonSqsReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }

        void ConfigureHostSettings(IAmazonSqsHostConfigurator configurator)
        {
            configurator.AccessKey(AccessKey);
            configurator.SecretKey(SecretKey);

            if (AmazonSqsConfig != null)
                configurator.Config(AmazonSqsConfig);

            if (AmazonSnsConfig != null)
                configurator.Config(AmazonSnsConfig);

            ConfigureAmazonSqsHost(configurator);
        }

        void CleanUpVirtualHost()
        {
            try
            {
                var settings = GetHostSettings();
                var connection = settings.CreateConnection();

                using (var amazonSqs = connection.CreateAmazonSqsClient())
                using (var amazonSns = connection.CreateAmazonSnsClient())
                {
                    CleanUpQueue(amazonSqs, "input_queue");

                    CleanUpQueue(amazonSqs, InputQueueName);

                    CleanupVirtualHost(amazonSqs, amazonSns);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void CleanUpQueue(IAmazonSQS amazonSqs, string queueName)
        {
            async Task CleanUpQueue(string queue)
            {
                try
                {
                    var queueUrl = (await amazonSqs.GetQueueUrlAsync(queue).ConfigureAwait(false)).QueueUrl;

                    await amazonSqs.PurgeQueueAsync(queueUrl).ConfigureAwait(false);
                }
                catch (QueueDoesNotExistException)
                {
                }
            }

            Task.Run(async () =>
            {
                await CleanUpQueue(queueName).ConfigureAwait(false);
                await CleanUpQueue($"{queueName}_skipped").ConfigureAwait(false);
                await CleanUpQueue($"{queueName}_error").ConfigureAwait(false);
            });
        }
    }
}
