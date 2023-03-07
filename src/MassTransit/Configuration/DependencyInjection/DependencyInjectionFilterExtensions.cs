namespace MassTransit
{
    using System;
    using Configuration;
    using Internals;


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
            UseConsumeFilter(configurator, filterType, provider, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedConsumePipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseConsumeFilter<TFilter>(this IConsumePipeConfigurator configurator, IServiceProvider provider)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type => filterType.HasInterface(typeof(ConsumeContext<>).MakeGenericType(type)));

            var observer = new ScopedConsumePipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);

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
            UseSendFilter(configurator, filterType, provider, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IServiceProvider provider,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedFilterSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseSendFilter<TFilter>(this ISendPipelineConfigurator configurator, IServiceProvider provider)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type => filterType.HasInterface(typeof(SendContext<>).MakeGenericType(type)));

            var observer = new ScopedFilterSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
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
            UsePublishFilter(configurator, filterType, provider, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IServiceProvider provider,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedFilterSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UsePublishFilter<TFilter>(this IPublishPipelineConfigurator configurator, IServiceProvider provider)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type => filterType.HasInterface(typeof(PublishContext<>).MakeGenericType(type)));

            var observer = new ScopedFilterSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
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
            UseExecuteActivityFilter(configurator, filterType, provider, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseExecuteActivityFilter<TFilter>(this IConsumePipeConfigurator configurator, IServiceProvider provider)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type => filterType.HasInterface(typeof(ExecuteContext<>).MakeGenericType(type)));

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
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
            UseCompensateActivityFilter(configurator, filterType, provider, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IServiceProvider provider,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseCompensateActivityFilter<TFilter>(this IConsumePipeConfigurator configurator, IServiceProvider provider)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type => filterType.HasInterface(typeof(CompensateContext<>).MakeGenericType(type)));

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, provider, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }
    }
}
