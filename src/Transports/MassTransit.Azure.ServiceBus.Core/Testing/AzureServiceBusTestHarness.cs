namespace MassTransit.Azure.ServiceBus.Core.Testing
{
    using System;
    using MassTransit.Testing;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class AzureServiceBusTestHarness :
        BusTestHarness
    {
        Uri _inputQueueAddress;

        public AzureServiceBusTestHarness(Uri serviceUri, string sharedAccessKeyName, string sharedAccessKeyValue, string inputQueueName = null)
        {
            if (serviceUri == null)
                throw new ArgumentNullException(nameof(serviceUri));

            HostAddress = serviceUri;
            SharedAccessKeyName = sharedAccessKeyName;
            SharedAccessKeyValue = sharedAccessKeyValue;

            TokenTimeToLive = TimeSpan.FromDays(1);
            TokenScope = TokenScope.Namespace;

            InputQueueName = inputQueueName ?? "input_queue";

            ConfigureMessageScheduler = true;
        }

        public string SharedAccessKeyName { get; }
        public string SharedAccessKeyValue { get; }
        public TimeSpan TokenTimeToLive { get; set; }
        public TokenScope TokenScope { get; set; }
        public override string InputQueueName { get; }
        public bool ConfigureMessageScheduler { get; set; }

        public override Uri InputQueueAddress => _inputQueueAddress;
        public Uri HostAddress { get; }

        public event Action<IServiceBusBusFactoryConfigurator> OnConfigureServiceBusBus;
        public event Action<IServiceBusReceiveEndpointConfigurator> OnConfigureServiceBusReceiveEndpoint;

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            OnConfigureServiceBusBus?.Invoke(configurator);
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            OnConfigureServiceBusReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.Host(HostAddress, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = SharedAccessKeyName;
                        s.SharedAccessKey = SharedAccessKeyValue;
                        s.TokenTimeToLive = TokenTimeToLive;
                        s.TokenScope = TokenScope;
                    });
                });

                ConfigureBus(x);

                ConfigureServiceBusBus(x);

                if (ConfigureMessageScheduler)
                    x.UseServiceBusMessageScheduler();

                x.ReceiveEndpoint(InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureServiceBusReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}
