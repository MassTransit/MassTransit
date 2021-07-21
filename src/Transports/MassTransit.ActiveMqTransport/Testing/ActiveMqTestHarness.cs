namespace MassTransit.ActiveMqTransport.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Configurators;
    using MassTransit.Testing;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class ActiveMqTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public ActiveMqTestHarness(string inputQueueName = null)
        {
            Username = "admin";
            Password = "admin";

            InputQueueName = inputQueueName ?? "input_queue";

            HostAddress = new Uri("activemq://localhost/");
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

        public string Username { get; set; }
        public string Password { get; set; }
        public int AdminPort { get; set; } = 8161;
        public string AdminPath { get; set; } = "api/jolokia/read/org.apache.activemq:type=Broker,brokerName=localhost";
        public bool CleanVirtualHost { get; set; } = true;
        public override string InputQueueName { get; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IActiveMqBusFactoryConfigurator> OnConfigureActiveMqBus;
        public event Action<IActiveMqReceiveEndpointConfigurator> OnConfigureActiveMqReceiveEndpoint;
        public event Action<IActiveMqHostConfigurator> OnConfigureActiveMqHost;
        public event Action<ISession> OnCleanupVirtualHost;

        protected virtual void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            OnConfigureActiveMqBus?.Invoke(configurator);
        }

        protected virtual void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            OnConfigureActiveMqReceiveEndpoint?.Invoke(configurator);
        }

        protected virtual void ConfigureActiveMqHost(IActiveMqHostConfigurator configurator)
        {
            OnConfigureActiveMqHost?.Invoke(configurator);
        }

        protected virtual void CleanupVirtualHost(ISession session)
        {
            OnCleanupVirtualHost?.Invoke(session);
        }

        protected virtual void ConfigureHost(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.Host(HostAddress, h =>
            {
                ConfigureHostSettings(h);
            });
        }

        void ConfigureHostSettings(IActiveMqHostConfigurator h)
        {
            h.Username(Username);
            h.Password(Password);

            ConfigureActiveMqHost(h);
        }

        public ActiveMqHostSettings GetHostSettings()
        {
            var host = new ActiveMqHostConfigurator(HostAddress);

            ConfigureHostSettings(host);

            return host.Settings;
        }

        public override async Task Clean()
        {
            if (AdminPort != 8161)
                return;

            var settings = GetHostSettings();

            using var connection = settings.CreateConnection();
            using var session = connection.CreateSession();

            (IList<string> queues, IList<string> topics) = await GetBrokerEntities();

            foreach (var entity in queues)
                session.DeleteDestination(SessionUtil.GetQueue(session, entity));

            foreach (var entity in topics)
                session.DeleteDestination(SessionUtil.GetTopic(session, entity));

            session.Close();
            connection.Close();

            CleanVirtualHost = false;
        }

        async Task<(IList<string>, IList<string>)> GetBrokerEntities()
        {
            using var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Add("Origin", "localhost");

            var requestUri = new UriBuilder("http", HostAddress.Host, AdminPort, "api/jolokia/read/org.apache.activemq:type=Broker,brokerName=localhost").Uri;
            using var response = await client.GetStreamAsync(requestUri);
            using var reader = new StreamReader(response);
            using var jsonReader = new JsonTextReader(reader);

            var queues = new List<string>();
            var topics = new List<string>();

            var token = await JToken.ReadFromAsync(jsonReader);

            var value = token["value"] as JContainer;
            var queue = value["Queues"] as JArray;

            IEnumerable<string> entities = from elements in queue.Children()
                select elements["objectName"].ToString();

            foreach (var entity in entities)
            {
                var parts = entity.Split(',');
                foreach (var part in parts)
                {
                    var keyValue = part.Split('=');
                    if (keyValue[0] == "destinationName")
                        queues.Add(keyValue[1]);
                }
            }

            var topic = value["Topics"] as JArray;

            entities = from elements in topic.Children()
                select elements["objectName"].ToString();

            foreach (var entity in entities)
            {
                var parts = entity.Split(',');
                foreach (var part in parts)
                {
                    var keyValue = part.Split('=');
                    if (keyValue[0] == "destinationName")
                        topics.Add(keyValue[1]);
                }
            }

            return (queues, topics);
        }

        protected override IBusControl CreateBus()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingActiveMq(x =>
            {
                ConfigureHost(x);

                ConfigureBus(x);

                ConfigureActiveMqBus(x);

                x.ReceiveEndpoint(InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureActiveMqReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });

            if (CleanVirtualHost)
                CleanUpVirtualHost();

            return busControl;
        }

        void CleanUpVirtualHost()
        {
            try
            {
                var settings = GetHostSettings();

                using var connection = settings.CreateConnection();
                using var model = connection.CreateSession();

                CleanUpQueue(model, "input_queue");

                if (InputQueueName != "input_queue")
                    CleanUpQueue(model, InputQueueName);

                CleanupVirtualHost(model);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void CleanUpQueue(ISession model, string queueName)
        {
            model.DeleteDestination(SessionUtil.GetQueue(model, queueName));
            model.DeleteDestination(SessionUtil.GetQueue(model, $"{queueName}_skipped"));
            model.DeleteDestination(SessionUtil.GetQueue(model, $"{queueName}_error"));
        }
    }
}
