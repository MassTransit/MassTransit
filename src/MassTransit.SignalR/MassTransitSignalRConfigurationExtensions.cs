namespace MassTransit.SignalR
{
    using System;
    using Configuration.Definitions;
    using Consumers;
    using Contracts;
    using Definition;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Scoping;


    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void AddSignalRHub<THub>(this IServiceCollectionBusConfigurator busConfigurator,
            Action<IHubLifetimeManagerOptions<THub>> configureHubLifetimeOptions = null)
            where THub : Hub
        {
            var options = new HubLifetimeManagerOptions<THub>();
            configureHubLifetimeOptions?.Invoke(options);

            busConfigurator.Collection.TryAddSingleton<IHubLifetimeScopeProvider, DependencyInjectionHubLifetimeScopeProvider>();

            busConfigurator.Collection.AddSingleton(provider => GetMassTransitHubLifetimeManager(provider, options));
            busConfigurator.Collection.AddSingleton<HubLifetimeManager<THub>>(sp => sp.GetRequiredService<MassTransitHubLifetimeManager<THub>>());

            busConfigurator.AddRequestClient<GroupManagement<THub>>(options.RequestTimeout);

            RegisterConsumers<THub>(busConfigurator);
        }

        static void RegisterConsumers<THub>(IServiceCollectionBusConfigurator busConfigurator)
            where THub : Hub
        {
            busConfigurator.Collection.AddSingleton<HubConsumerDefinition<THub>>();

            busConfigurator.Collection.TryAddSingleton<IConsumerDefinition<AllConsumer<THub>>, AllConsumerDefinition<THub>>();
            busConfigurator.Collection.TryAddSingleton<IConsumerDefinition<ConnectionConsumer<THub>>, ConnectionConsumerDefinition<THub>>();
            busConfigurator.Collection.TryAddSingleton<IConsumerDefinition<GroupConsumer<THub>>, GroupConsumerDefinition<THub>>();
            busConfigurator.Collection.TryAddSingleton<IConsumerDefinition<GroupManagementConsumer<THub>>, GroupManagementConsumerDefinition<THub>>();
            busConfigurator.Collection.TryAddSingleton<IConsumerDefinition<UserConsumer<THub>>, UserConsumerDefinition<THub>>();

            busConfigurator.AddConsumer<AllConsumer<THub>>();
            busConfigurator.AddConsumer<ConnectionConsumer<THub>>();
            busConfigurator.AddConsumer<GroupConsumer<THub>>();
            busConfigurator.AddConsumer<GroupManagementConsumer<THub>>();
            busConfigurator.AddConsumer<UserConsumer<THub>>();
        }

        static MassTransitHubLifetimeManager<THub> GetMassTransitHubLifetimeManager<THub>(IServiceProvider provider, HubLifetimeManagerOptions<THub> options)
            where THub : Hub
        {
            var scopeProvider = provider.GetRequiredService<IHubLifetimeScopeProvider>();
            var resolver = provider.GetRequiredService<IHubProtocolResolver>();
            return new MassTransitHubLifetimeManager<THub>(options, scopeProvider, resolver);
        }
    }
}
