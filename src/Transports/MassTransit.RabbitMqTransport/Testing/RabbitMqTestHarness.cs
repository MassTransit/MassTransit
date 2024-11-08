namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;
    using Serialization;
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
        public event Func<IChannel, Task> OnCleanupVirtualHost;

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

        protected virtual Task CleanupVirtualHost(IChannel channel)
        {
            return OnCleanupVirtualHost != null ? OnCleanupVirtualHost(channel) : Task.CompletedTask;
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

        public override async Task Clean()
        {
            var settings = GetHostSettings();

            var connectionFactory = settings.GetConnectionFactory();

            await using var connection = settings.EndpointResolver != null
                ? await connectionFactory.CreateConnectionAsync(settings.EndpointResolver, settings.Host)
                : await connectionFactory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            IList<string> exchanges = await GetVirtualHostEntities("exchanges").ConfigureAwait(false);
            foreach (var exchange in exchanges)
                await channel.ExchangeDeleteAsync(exchange);

            IList<string> queues = await GetVirtualHostEntities("queues").ConfigureAwait(false);
            foreach (var queue in queues)
                await channel.QueueDeleteAsync(queue);

            await channel.CloseAsync();

            CleanVirtualHost = false;
        }

        async Task<IList<string>> GetVirtualHostEntities(string element)
        {
            using var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var requestUri = new UriBuilder("http", HostAddress.Host, 15672, $"api/{element}/{HostAddress.AbsolutePath.Trim('/')}").Uri;

            var bytes = await client.GetByteArrayAsync(requestUri);

            var rootElement = JsonSerializer.Deserialize<JsonElement>(bytes, SystemTextJsonMessageSerializer.Options);

            var entities = rootElement.EnumerateArray().Select(x => x.GetProperty("name").GetString()).ToArray();

            return entities.Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("amq.")).ToList();
        }

        protected override async Task<IBusControl> CreateBus()
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
                await CleanUpVirtualHost();

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

        async Task CleanUpVirtualHost()
        {
            try
            {
                var settings = GetHostSettings();

                var connectionFactory = settings.GetConnectionFactory();

                await using var connection = settings.EndpointResolver != null
                    ? await connectionFactory.CreateConnectionAsync(settings.EndpointResolver, settings.Host)
                    : await connectionFactory.CreateConnectionAsync();

                await using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeleteAsync("input_queue");
                await channel.QueueDeleteAsync("input_queue");

                await channel.ExchangeDeleteAsync("input_queue_skipped");
                await channel.QueueDeleteAsync("input_queue_skipped");

                await channel.ExchangeDeleteAsync("input_queue_error");
                await channel.QueueDeleteAsync("input_queue_error");

                await channel.ExchangeDeleteAsync("input_queue_delay");

                if (InputQueueName != "input_queue")
                {
                    await channel.ExchangeDeleteAsync(InputQueueName);
                    await channel.QueueDeleteAsync(InputQueueName);

                    await channel.ExchangeDeleteAsync(InputQueueName + "_skipped");
                    await channel.QueueDeleteAsync(InputQueueName + "_skipped");

                    await channel.ExchangeDeleteAsync(InputQueueName + "_error");
                    await channel.QueueDeleteAsync(InputQueueName + "_error");

                    await channel.ExchangeDeleteAsync(InputQueueName + "_delay");
                }

                await CleanupVirtualHost(channel);

                await channel.CloseAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
