namespace MassTransit
{
    using System;
    using Autofac;
    using AutofacIntegration;
    using AutofacIntegration.Filters;
    using Courier;


    public static class AutofacFilterExtensions
    {
        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="lifetimeScopeProvider">Lifetime Scope Provider</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType,
            ILifetimeScopeProvider lifetimeScopeProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeScopeProvider));

            configurator.ConnectConsumerConfigurationObserver(new ScopedConsumerConsumePipeSpecificationObserver(filterType, lifetimeScopeProvider));
            configurator.ConnectSagaConfigurationObserver(new ScopedSagaConsumePipeSpecificationObserver(filterType, lifetimeScopeProvider));
        }

        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
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

            var lifetimeScope = registration.GetRequiredService<ILifetimeScope>();
            ILifetimeScopeProvider lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            configurator.UseConsumeFilter(filterType, lifetimeScopeProvider);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="lifetimeScopeProvider">Lifetime Scope Provider</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeScopeProvider));

            var observer = new ScopedSendPipeSpecificationObserver(filterType, lifetimeScopeProvider);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
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

            var lifetimeScope = registration.GetRequiredService<ILifetimeScope>();
            ILifetimeScopeProvider lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            configurator.UseSendFilter(filterType, lifetimeScopeProvider);
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="lifetimeScopeProvider">Lifetime Scope Provider</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeScopeProvider));

            var observer = new ScopedPublishPipeSpecificationObserver(filterType, lifetimeScopeProvider);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
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

            var lifetimeScope = registration.GetRequiredService<ILifetimeScope>();
            ILifetimeScopeProvider lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            configurator.UsePublishFilter(filterType, lifetimeScopeProvider);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="lifetimeScopeProvider">Lifetime Scope Provider</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType,
            ILifetimeScopeProvider lifetimeScopeProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeScopeProvider));

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, lifetimeScopeProvider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{T}" />
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

            var lifetimeScope = registration.GetRequiredService<ILifetimeScope>();
            ILifetimeScopeProvider lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            configurator.UseExecuteActivityFilter(filterType, lifetimeScopeProvider);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="lifetimeScopeProvider">Lifetime Scope Provider</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType,
            ILifetimeScopeProvider lifetimeScopeProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeScopeProvider));

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, lifetimeScopeProvider);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
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

            var lifetimeScope = registration.GetRequiredService<ILifetimeScope>();
            ILifetimeScopeProvider lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            configurator.UseCompensateActivityFilter(filterType, lifetimeScopeProvider);
        }
    }
}
