namespace MassTransit.SignalR
{
    using System;
    using Consumers;
    using Contracts;
    using Definition;
    using ExtensionsDependencyInjectionIntegration;
    using MessageData;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;


    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void AddSignalRHubConsumers<THub>(this IServiceCollectionConfigurator configurator,
            Action<IHubLifetimeManagerOptions<THub>> configureHubLifetimeOptions = null)
            where THub : Hub
        {
            var options = new HubLifetimeManagerOptions<THub>();
            configureHubLifetimeOptions?.Invoke(options);

            configurator.Collection.AddSingleton(options);
            configurator.Collection.AddSingleton(GetLifetimeManager<THub>);

            configurator.AddRequestClient<GroupManagement<THub>>(options.RequestTimeout);

            //TODO: check if use options.UseMessageData
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

        static HubLifetimeManager<THub> GetLifetimeManager<THub>(IServiceProvider provider)
            where THub : Hub
        {
            var options = provider.GetService<MassTransitSignalROptions>() ?? new MassTransitSignalROptions();
            var hubLifetimeManagerOptions = provider.GetRequiredService<HubLifetimeManagerOptions<THub>>();
            var bus = provider.GetRequiredService<IBus>();
            var hubProtocolResolver = provider.GetRequiredService<IHubProtocolResolver>();

            if (options.UseMessageData || hubLifetimeManagerOptions.UseMessageData)
                return new MassTransitMessageDataHubLifetimeManager<THub>(hubLifetimeManagerOptions, bus, hubProtocolResolver,
                    provider.GetRequiredService<IMessageDataRepository>());
            return new MassTransitHubLifetimeManager<THub>(hubLifetimeManagerOptions, bus, hubProtocolResolver);
        }

        public static void AddSignalRHubEndpoints<THub>(this IBusFactoryConfigurator configurator,
            IServiceProvider serviceProvider,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
            where THub : Hub
        {
            // Get the configuration options
            var options = serviceProvider.GetService<MassTransitSignalROptions>() ?? new MassTransitSignalROptions();
            var hubLifetimeManagerOptions = serviceProvider.GetRequiredService<HubLifetimeManagerOptions<THub>>();
            var endpointDefinition = new HubEndpointDefinition<THub>();

            if (options.UseMessageData || hubLifetimeManagerOptions.UseMessageData)
            {
                configurator.UseMessageData(serviceProvider.GetService<IMessageDataRepository>());
                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<AllMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<ConnectionMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<GroupMessageDataConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<UserMessageDataConsumer<THub>>(serviceProvider);
                });
            }
            else
            {
                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<AllConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<ConnectionConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<GroupConsumer<THub>>(serviceProvider);
                });

                configurator.ReceiveEndpoint(endpointDefinition, null, e =>
                {
                    configureEndpoint?.Invoke(e);

                    e.ConfigureConsumer<UserConsumer<THub>>(serviceProvider);
                });
            }

            // Common Receive Endpoint
            configurator.ReceiveEndpoint(endpointDefinition, null, e =>
            {
                configureEndpoint?.Invoke(e);

                e.ConfigureConsumer<GroupManagementConsumer<THub>>(serviceProvider);
            });
        }


        class HubEndpointDefinition<THub> :
            DefaultEndpointDefinition
            where THub : Hub
        {
            readonly Lazy<string> _hubName = new Lazy<string>(() => typeof(THub).Name);
            public HubEndpointDefinition()
                : base(true)
            {
            }

            public override string GetEndpointName(IEndpointNameFormatter formatter)
            {
                return formatter.TemporaryEndpoint($"signalr_{_hubName.Value}");
            }
        }
    }
}
