namespace MassTransit.ActiveMqTransport.Testing
{
    using System;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Configurators;
    using MassTransit.Testing;


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

            CleanUpVirtualHost();

            return busControl;
        }

        void CleanUpVirtualHost()
        {
            try
            {
                var settings = GetHostSettings();

                using (var connection = settings.CreateConnection())
                using (var model = connection.CreateSession())
                {
                    CleanUpQueue(model, "input_queue");

                    CleanUpQueue(model, InputQueueName);

                    CleanupVirtualHost(model);
                }
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
