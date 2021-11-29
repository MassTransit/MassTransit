namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class ObserverRegistrationExtensions
    {
        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddBusObserver<T>(this IServiceCollection services)
            where T : class, IBusObserver
        {
            services.TryAddSingleton<IBusObserver, T>();
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public static IServiceCollection AddBusObserver(this IServiceCollection services, Func<IServiceProvider, IBusObserver> factoryMethod)
        {
            services.TryAddSingleton(factoryMethod);
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddReceiveEndpointObserver<T>(this IServiceCollection services)
            where T : class, IReceiveEndpointObserver
        {
            services.TryAddSingleton<IReceiveEndpointObserver, T>();
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public static IServiceCollection AddReceiveEndpointObserver(this IServiceCollection services,
            Func<IServiceProvider, IReceiveEndpointObserver> factoryMethod)
        {
            services.TryAddSingleton(factoryMethod);
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddReceiveObserver<T>(this IServiceCollection services)
            where T : class, IReceiveObserver
        {
            services.TryAddSingleton<IReceiveObserver, T>();
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public static IServiceCollection AddReceiveObserver(this IServiceCollection services, Func<IServiceProvider, IReceiveObserver> factoryMethod)
        {
            services.TryAddSingleton(factoryMethod);
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddConsumeObserver<T>(this IServiceCollection services)
            where T : class, IConsumeObserver
        {
            services.TryAddSingleton<IConsumeObserver, T>();
            return services;
        }

        /// <summary>
        /// Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsumeObserver(this IServiceCollection services, Func<IServiceProvider, IConsumeObserver> factoryMethod)
        {
            services.TryAddSingleton(factoryMethod);
            return services;
        }
    }
}
