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
        public static void AddSignalRHub<THub>(this IServiceCollectionConfigurator configurator,
            Action<IHubLifetimeManagerOptions<THub>> configureHubLifetimeOptions = null)
            where THub : Hub
        {
            var options = new HubLifetimeManagerOptions<THub>();
            configureHubLifetimeOptions?.Invoke(options);

            configurator.Collection.TryAddSingleton<IHubLifetimeScopeProvider, DependencyInjectionHubLifetimeScopeProvider>();

            configurator.Collection.AddSingleton(provider => GetMassTransitHubLifetimeManager(provider, options));
            configurator.Collection.AddSingleton<HubLifetimeManager<THub>>(sp => sp.GetRequiredService<MassTransitHubLifetimeManager<THub>>());

            configurator.AddRequestClient<GroupManagement<THub>>(options.RequestTimeout);

            RegisterConsumers<THub>(configurator);
        }

        static void RegisterConsumers<THub>(IServiceCollectionConfigurator configurator)
            where THub : Hub
        {
            configurator.Collection.AddSingleton<HubConsumerDefinition<THub>>();

            configurator.Collection.TryAddSingleton<IConsumerDefinition<AllConsumer<THub>>, AllConsumerDefinition<THub>>();
            configurator.Collection.TryAddSingleton<IConsumerDefinition<ConnectionConsumer<THub>>, ConnectionConsumerDefinition<THub>>();
            configurator.Collection.TryAddSingleton<IConsumerDefinition<GroupConsumer<THub>>, GroupConsumerDefinition<THub>>();
            configurator.Collection.TryAddSingleton<IConsumerDefinition<GroupManagementConsumer<THub>>, GroupManagementConsumerDefinition<THub>>();
            configurator.Collection.TryAddSingleton<IConsumerDefinition<UserConsumer<THub>>, UserConsumerDefinition<THub>>();

            configurator.AddConsumer<AllConsumer<THub>>();
            configurator.AddConsumer<ConnectionConsumer<THub>>();
            configurator.AddConsumer<GroupConsumer<THub>>();
            configurator.AddConsumer<GroupManagementConsumer<THub>>();
            configurator.AddConsumer<UserConsumer<THub>>();
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
