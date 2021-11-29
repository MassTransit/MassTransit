namespace MassTransit
{
    using System;
    using Configuration;


    public static class DependencyInjectionFilterExtensions
    {
        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new ScopedConsumePipeSpecificationObserver(filterType, provider);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new ScopedFilterSpecificationObserver(filterType, provider);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new ScopedFilterSpecificationObserver(filterType, provider);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, provider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, provider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }
    }
}
