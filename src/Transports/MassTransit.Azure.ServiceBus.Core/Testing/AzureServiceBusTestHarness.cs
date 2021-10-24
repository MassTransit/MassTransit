namespace MassTransit.Azure.ServiceBus.Core.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configurators;
    using Context;
    using global::Azure;
    using global::Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Testing;


    public class AzureServiceBusTestHarness :
        BusTestHarness
    {
        Uri _inputQueueAddress;

        public AzureServiceBusTestHarness(Uri serviceUri, AzureNamedKeyCredential namedKeyCredential, string inputQueueName = null)
        {
            if (serviceUri == null)
                throw new ArgumentNullException(nameof(serviceUri));

            HostAddress = serviceUri;
            NamedKeyCredential = namedKeyCredential;

            InputQueueName = inputQueueName ?? "input_queue";

            ConfigureMessageScheduler = true;
        }

        public AzureNamedKeyCredential NamedKeyCredential { get; }
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

        public override async Task Clean()
        {
            var managementClient = CreateManagementClient();

            async Task<List<T>> ToListAsync<T>(AsyncPageable<T> pageable)
            {
                var items = new List<T>();
                await foreach (var item in pageable)
                {
                    items.Add(item);
                }

                return items;
            }

            var pageableTopics = managementClient.GetTopicsAsync();
            var topics = await ToListAsync(pageableTopics);
            while (topics.Count > 0)
            {
                foreach (var topic in topics)
                    await managementClient.DeleteTopicAsync(topic.Name);

                await Task.Delay(500);

                topics = await ToListAsync(managementClient.GetTopicsAsync());
            }

            var pageableQueues = managementClient.GetQueuesAsync();
            var queues = await ToListAsync(pageableQueues);
            while (queues.Count > 0)
            {
                foreach (var queue in queues)
                    await managementClient.DeleteQueueAsync(queue.Name);

                await Task.Delay(500);

                queues = await ToListAsync(managementClient.GetQueuesAsync());
            }
        }

        ServiceBusAdministrationClient CreateManagementClient()
        {
            var endpoint = new UriBuilder(HostAddress) {Path = ""}.Uri.ToString();

            return new ServiceBusAdministrationClient(endpoint, NamedKeyCredential);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.Host(HostAddress, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = NamedKeyCredential;
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
