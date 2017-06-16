using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MassTransit.Pipeline;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    public class MassTransitOptions
    {
        private readonly IServiceCollection _services;
        private readonly ConcurrentDictionary<Type, ICachedConfigurator> _consumerHandlers =
                new ConcurrentDictionary<Type, ICachedConfigurator>();
        public MassTransitOptions(IServiceCollection services)
        {
            _services = services;
        }

        public void AddConsumer<T>()
            where T : class, IConsumer
        {
            _services.AddScoped<T>();   

            _consumerHandlers.GetOrAdd(typeof(T), _ => new CachedConfigurator<T>());
        }

        internal IEnumerable<ICachedConfigurator> GetConsumerHandlers()
        {
            return _consumerHandlers.Values.ToList();
        }

        class CachedConfigurator<T> : ICachedConfigurator
            where T : class, IConsumer
        {
            public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider services)
            {
                configurator.Consumer(new ExtensionsDependencyInjectionConsumerFactory<T>(services));
            }
        }
    }
}