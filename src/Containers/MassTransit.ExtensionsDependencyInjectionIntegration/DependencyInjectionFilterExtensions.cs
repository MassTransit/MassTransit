namespace MassTransit
{
    using System;
    using Courier;
    using ExtensionsDependencyInjectionIntegration.Filters;
    using Registration;


    public static class DependencyInjectionFilterExtensions
    {
        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IConfigurationServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var serviceProvider = provider.GetRequiredService<IServiceProvider>();

            configurator.ConnectConsumerConfigurationObserver(new ScopedConsumerConsumePipeSpecificationObserver(filterType, serviceProvider));
            configurator.ConnectSagaConfigurationObserver(new ScopedSagaConsumePipeSpecificationObserver(filterType, serviceProvider));
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IConfigurationServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var serviceProvider = provider.GetRequiredService<IServiceProvider>();
            var observer = new ScopedSendPipeSpecificationObserver(filterType, serviceProvider);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IConfigurationServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var serviceProvider = provider.GetRequiredService<IServiceProvider>();
            var observer = new ScopedPublishPipeSpecificationObserver(filterType, serviceProvider);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IConfigurationServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var serviceProvider = provider.GetRequiredService<IServiceProvider>();
            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, serviceProvider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IConfigurationServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var serviceProvider = provider.GetRequiredService<IServiceProvider>();
            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, serviceProvider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }
    }
}
