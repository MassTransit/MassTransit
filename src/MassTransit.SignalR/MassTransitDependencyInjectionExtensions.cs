namespace MassTransit.SignalR
{
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class MassTransitDependencyInjectionExtensions
    {
        public static void AddMassTransitBackplane(this IServiceCollection serviceCollection, out IReadOnlyDictionary<Type, IReadOnlyList<Type>> hubConsumers, params Assembly[] hubAssemblies)
        {
            var dict = new Dictionary<Type, IReadOnlyList<Type>>();

            var hubs = from a in hubAssemblies
                       from t in a.GetTypes()
                       where typeof(Hub).IsAssignableFrom(t)
                       select t;

            var consumers = (from t in typeof(MassTransitDependencyInjectionExtensions).Assembly.GetTypes()
                            where typeof(IConsumer).IsAssignableFrom(t)
                            select t).ToArray();

            foreach (var hub in hubs)
            {
                var consumerTypes = new List<Type>(consumers.Length);

                foreach (var consumer in consumers)
                {
                    var consumerType = consumer.MakeGenericType(hub);
                    consumerTypes.Add(consumerType);
                    serviceCollection.AddScoped(consumerType);
                }

                dict.Add(hub, consumerTypes);
            }

            hubConsumers = dict;

            serviceCollection.AddSingleton(typeof(HubLifetimeManager<>), typeof(MassTransitHubLifetimeManager<>));
        }
    }
}
