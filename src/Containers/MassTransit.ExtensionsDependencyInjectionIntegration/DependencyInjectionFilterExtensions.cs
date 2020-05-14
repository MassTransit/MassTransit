namespace MassTransit
{
    using System;
    using Courier;
    using ExtensionsDependencyInjectionIntegration.Filters;


    public static class DependencyInjectionFilterExtensions
    {
        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="registration">Registration Context</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistration registration)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var provider = registration.GetRequiredService<IServiceProvider>();
            var observer = new ScopedConsumePipeSpecificationObserver(configurator, filterType, provider);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="registration">Registration Context</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IRegistration registration)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var provider = registration.GetRequiredService<IServiceProvider>();
            var observer = new ScopedSendPipeSpecificationObserver(filterType, provider);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="registration">Registration Context</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IRegistration registration)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var provider = registration.GetRequiredService<IServiceProvider>();
            var observer = new ScopedPublishPipeSpecificationObserver(filterType, provider);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="registration">Registration Context</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistration registration)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var provider = registration.GetRequiredService<IServiceProvider>();
            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, provider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="registration">Registration Context</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistration registration)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var provider = registration.GetRequiredService<IServiceProvider>();
            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, provider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }
    }
}
