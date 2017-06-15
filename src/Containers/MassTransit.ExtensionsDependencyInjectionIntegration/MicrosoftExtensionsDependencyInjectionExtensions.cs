using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.ConsumeConfigurators;
using MassTransit.MicrosoftExtensionsDependencyInjectionIntegration;
using System.Collections.Generic;
using MassTransit.Saga;

namespace MassTransit
{
    public static class ExtensionsDependencyInjectionIntegration
    {
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider services)
        {
            foreach(var consumer in ConsumerConfiguratorCache.GetConsumers())
            {
                ConsumerConfiguratorCache.Configure(consumer, configurator, services);
            }

            foreach(var saga in SagaConfiguratorCache.GetSagas())
            {
                SagaConfiguratorCache.Configure(saga, configurator, services);
            }
        }

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IServiceProvider services, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var factory = new MicrosoftExtensionsDependencyInjectionConsumerFactory<T>(services);
            configurator.Consumer(factory, configure);
        }

        public static void AddMassTransit(this IServiceCollection serviceCollection, Action<MassTransitOptions> opt = null)
        {            
            var options = new MassTransitOptions(serviceCollection);
            serviceCollection.AddSingleton(options);

            if(opt == null)
            opt(options);
        }

        public static void AddMassTransit(this IServiceCollection services, params Assembly[] assemblies)
        {
            AddRequiredServices(services);
            AddHandlers(services, assemblies);
        }

        public static void AddMassTransit(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            AddRequiredServices(services);
            AddHandlers(services, assemblies);
        }

        public static void AddMassTransit(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
        {
            AddRequiredServices(services);
            AddHandlers(services, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static void AddMassTransit(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes)
        {
            AddRequiredServices(services);
            AddHandlers(services, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        private static void AddRequiredServices(IServiceCollection services)
        {
            
        }

        private static void AddHandlers(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            foreach(var type in assembliesToScan.SelectMany(a => a.ExportedTypes))
            {
                if(type.CanBeCastTo(typeof(ISaga)))
                {
                    services.AddScoped(type);
                    SagaConfiguratorCache.Cache(type);
                }
                else if(type.CanBeCastTo(typeof(IConsumer)))
                {
                    services.AddScoped(type);
                    ConsumerConfiguratorCache.Cache(type);
                }
            }
        }

        private static bool CanBeCastTo(this Type handlerType, Type interfaceType)
        {
            if (handlerType == null) return false;

            if (handlerType == interfaceType) return true;

            return interfaceType.GetTypeInfo().IsAssignableFrom(handlerType.GetTypeInfo());
        }
    }
}