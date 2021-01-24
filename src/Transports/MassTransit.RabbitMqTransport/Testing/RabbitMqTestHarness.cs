namespace MassTransit.RabbitMqTransport.Testing
{
    using System;
    using Configurators;
    using MassTransit.Testing;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public RabbitMqTestHarness(string inputQueueName = null)
        {
            Username = "guest";
            Password = "guest";

            InputQueueName = inputQueueName ?? "input_queue";

            NameFormatter = new RabbitMqMessageNameFormatter();

            HostAddress = new Uri("rabbitmq://localhost/test/");
        }

        public Uri HostAddress
        {
            get => _hostAddress;
            set
            {
                _hostAddress = value;
                _inputQueueAddress = new Uri($"queue:{InputQueueName}");
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public bool CleanVirtualHost { get; set; } = true;
        public override string InputQueueName { get; }
        public string NodeHostName { get; set; }
        public IMessageNameFormatter NameFormatter { get; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IRabbitMqBusFactoryConfigurator> OnConfigureRabbitMqBus;
        public event Action<IRabbitMqReceiveEndpointConfigurator> OnConfigureRabbitMqReceiveEndpoint;
        public event Action<IRabbitMqHostConfigurator> OnConfigureRabbitMqHost;
        public event Action<IModel> OnCleanupVirtualHost;

        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            OnConfigureRabbitMqBus?.Invoke(configurator);
        }

        protected virtual void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            OnConfigureRabbitMqReceiveEndpoint?.Invoke(configurator);
        }

        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
            OnConfigureRabbitMqHost?.Invoke(configurator);
        }

        protected virtual void CleanupVirtualHost(IModel model)
        {
            OnCleanupVirtualHost?.Invoke(model);
        }

        protected virtual void ConfigureHost(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host(HostAddress, h =>
            {
                ConfigureHostSettings(h);
            });
        }

        public RabbitMqHostSettings GetHostSettings()
        {
            var host = new RabbitMqHostConfigurator(HostAddress);

            ConfigureHostSettings(host);

            return host.Settings;
        }

        protected override IBusControl CreateBus()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                ConfigureHost(x);

                ConfigureBus(x);

                ConfigureRabbitMqBus(x);

                x.ReceiveEndpoint(InputQueueName, e =>
                {
                    e.PrefetchCount = 16;
                    e.PurgeOnStartup = true;

                    ConfigureReceiveEndpoint(e);

                    ConfigureRabbitMqReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });

            if (CleanVirtualHost)
                CleanUpVirtualHost();

            return busControl;
        }

        void ConfigureHostSettings(IRabbitMqHostConfigurator configurator)
        {
            configurator.Username(Username);
            configurator.Password(Password);

            if (!string.IsNullOrWhiteSpace(NodeHostName))
                configurator.UseCluster(c => c.Node(NodeHostName));

            ConfigureRabbitMqHost(configurator);
        }

        void CleanUpVirtualHost()
        {
            try
            {
                var settings = GetHostSettings();

                var connectionFactory = settings.GetConnectionFactory();

                using var connection = settings.EndpointResolver != null
                    ? connectionFactory.CreateConnection(settings.EndpointResolver, settings.Host)
                    : connectionFactory.CreateConnection();

                using var model = connection.CreateModel();

                model.ExchangeDelete("input_queue");
                model.QueueDelete("input_queue");

                model.ExchangeDelete("input_queue_skipped");
                model.QueueDelete("input_queue_skipped");

                model.ExchangeDelete("input_queue_error");
                model.QueueDelete("input_queue_error");

                model.ExchangeDelete("input_queue_delay");

                if (InputQueueName != "input_queue")
                {
                    model.ExchangeDelete(InputQueueName);
                    model.QueueDelete(InputQueueName);

                    model.ExchangeDelete(InputQueueName + "_skipped");
                    model.QueueDelete(InputQueueName + "_skipped");

                    model.ExchangeDelete(InputQueueName + "_error");
                    model.QueueDelete(InputQueueName + "_error");

                    model.ExchangeDelete(InputQueueName + "_delay");
                }

                CleanupVirtualHost(model);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
