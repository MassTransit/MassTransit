namespace MassTransit
{
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
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBusObserver, T>());
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
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IReceiveEndpointObserver, T>());
            return services;
        }

        /// <summary>
        /// Add a receive  observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddReceiveObserver<T>(this IServiceCollection services)
            where T : class, IReceiveObserver
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IReceiveObserver, T>());
            return services;
        }

        /// <summary>
        /// Add a consume observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddConsumeObserver<T>(this IServiceCollection services)
            where T : class, IConsumeObserver
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConsumeObserver, T>());
            return services;
        }

        /// <summary>
        /// Add a send observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddSendObserver<T>(this IServiceCollection services)
            where T : class, ISendObserver
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISendObserver, T>());
            return services;
        }

        /// <summary>
        /// Add a publish observer to the container, which will be resolved and connected to the bus by the container
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddPublishObserver<T>(this IServiceCollection services)
            where T : class, IPublishObserver
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPublishObserver, T>());
            return services;
        }
    }
}
