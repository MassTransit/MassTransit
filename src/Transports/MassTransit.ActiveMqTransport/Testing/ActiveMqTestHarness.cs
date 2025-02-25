﻿namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using ActiveMqTransport.Configuration;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Serialization;


    public class ActiveMqTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public ActiveMqTestHarness(string protocol = ActiveMqHostAddress.ActiveMqScheme, string inputQueueName = null)
        {
            Username = "admin";
            Password = "admin";

            InputQueueName = inputQueueName ?? "input_queue";

            if (protocol == ActiveMqHostAddress.AmqpScheme)
                HostAddress = new Uri("amqp://localhost:5672");
            else
                HostAddress = new Uri("activemq://localhost:61616");
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
            var address = HostAddress;
            if (HostAddress.Scheme == ActiveMqHostAddress.AmqpScheme)
                address = new UriBuilder(ActiveMqHostAddress.ActiveMqScheme, HostAddress.Host, HostAddress.Port).Uri;
            var host = new ActiveMqHostConfigurator(address);

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

            var bytes = await client.GetByteArrayAsync(requestUri);

            var element = JsonSerializer.Deserialize<JsonElement>(bytes, SystemTextJsonMessageSerializer.Options).GetProperty("value");

            var queuesElement = element.GetProperty("Queues");

            List<string> queues = queuesElement.EnumerateArray().Select(x => GetDestinationName(x.GetProperty("objectName").GetString())).ToList();

            var topicsElement = element.GetProperty("Topics");

            List<string> topics = topicsElement.EnumerateArray().Select(x => GetDestinationName(x.GetProperty("objectName").GetString())).ToList();

            return (queues, topics);
        }

        string GetDestinationName(string input)
        {
            return input.Split(',').Select(part => part.Split('='))
                .Where(keyValue => keyValue[0] == "destinationName")
                .Select(keyValue => keyValue[1])
                .FirstOrDefault();
        }

        protected override async Task<IBusControl> CreateBus()
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

        static void CleanUpQueue(ISession session, string queueName)
        {
            DeleteDestination(session, queueName);
            DeleteDestination(session, $"{queueName}_skipped");
            DeleteDestination(session, $"{queueName}_error");
        }

        static void DeleteDestination(ISession session, string destinationName)
        {
            var destination = SessionUtil.GetQueue(session, destinationName);
            session.DeleteDestination(destination);
        }
    }
}
