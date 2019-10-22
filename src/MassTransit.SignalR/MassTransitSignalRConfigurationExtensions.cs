namespace MassTransit.SignalR
{
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using Definition;
    using MassTransit.SignalR.Consumers;
    using MassTransit.SignalR.Contracts;
    using MassTransit.MessageData;

    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void AddSignalRHubConsumers<THub>(this IServiceCollectionConfigurator configurator)
            where THub : Hub
        {
            // Add Registrations for Regular Consumers
            configurator.AddConsumer<AllConsumer<THub>>();
            configurator.AddConsumer<ConnectionConsumer<THub>>();
            configurator.AddConsumer<GroupConsumer<THub>>();
            configurator.AddConsumer<GroupManagementConsumer<THub>>();
            configurator.AddConsumer<UserConsumer<THub>>();

            // Add Registrations for Message Data Consumers
            configurator.AddConsumer<AllMessageDataConsumer<THub>>();
            configurator.AddConsumer<ConnectionMessageDataConsumer<THub>>();
            configurator.AddConsumer<GroupMessageDataConsumer<THub>>();
            configurator.AddConsumer<GroupManagementConsumer<THub>>();
            configurator.AddConsumer<UserMessageDataConsumer<THub>>();
        }

        public static void AddSignalRHubEndpoints<THub>(
            this IBusFactoryConfigurator configurator,
            IServiceProvider serviceProvider,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
            where THub : Hub
        {
            // Get the configuration options
            var options = serviceProvider.GetService(typeof(MassTransitSignalROptions)) as MassTransitSignalROptions;

            if (!options.UseMessageData)
            {
                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<AllConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<ConnectionConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<GroupConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<UserConsumer<THub>>(serviceProvider);
                });
            }
            else
            {
                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<AllMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<ConnectionMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<GroupMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    e.UseMessageData(serviceProvider.GetService(typeof(IMessageDataRepository)) as IMessageDataRepository);
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<UserMessageDataConsumer<THub>>(serviceProvider);
                });
            }

            // Common Receive Endpoint
            configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
            {
                configureEndpoint?.Invoke(e);

                e.ConfigureConsumer<GroupManagementConsumer<THub>>(serviceProvider);
            });
        }

        class HubEndpointDefinition<THub> :
            DefaultEndpointDefinition
            where THub : Hub
        {
            public HubEndpointDefinition()
                : base(true)
            {
            }

            public override string GetEndpointName(IEndpointNameFormatter formatter)
            {
                return formatter.TemporaryEndpoint($"signalr_{typeof(THub).Name}");
            }
        }
    }
}
