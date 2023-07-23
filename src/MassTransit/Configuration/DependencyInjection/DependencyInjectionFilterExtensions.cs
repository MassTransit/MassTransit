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
        /// <param name="context">Configuration registration context</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
        {
            UseConsumeFilter(configurator, filterType, context, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseConsumeFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedConsumePipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ConsumeContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration registration context</param>
        public static void UseConsumeFilter<TFilter>(this IConsumePipeConfigurator configurator, IRegistrationContext context)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var filterType = typeof(TFilter);

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            messageTypeFilterConfigurator.Include(type =>
                typeof(IFilter<>).MakeGenericType(typeof(ConsumeContext<>).MakeGenericType(type)).IsAssignableFrom(filterType));

            var observer = new ScopedConsumePipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IRegistrationContext context)
        {
            UseSendFilter(configurator, filterType, context, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseSendFilter(this ISendPipelineConfigurator configurator, Type filterType, IRegistrationContext context,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedFilterSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="SendContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration registration context</param>
        public static void UseSendFilter<TFilter>(this ISendPipelineConfigurator configurator, IRegistrationContext context)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();

            messageTypeFilterConfigurator.Include(type =>
                typeof(IFilter<>).MakeGenericType(typeof(SendContext<>).MakeGenericType(type)).IsAssignableFrom(filterType));

            var observer = new ScopedFilterSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IRegistrationContext context)
        {
            UsePublishFilter(configurator, filterType, context, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UsePublishFilter(this IPublishPipelineConfigurator configurator, Type filterType, IRegistrationContext context,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedFilterSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="PublishContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration registration context</param>
        public static void UsePublishFilter<TFilter>(this IPublishPipelineConfigurator configurator, IRegistrationContext context)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();

            messageTypeFilterConfigurator.Include(type =>
                typeof(IFilter<>).MakeGenericType(typeof(PublishContext<>).MakeGenericType(type)).IsAssignableFrom(filterType));

            var observer = new ScopedFilterSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(observer));
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
        {
            UseExecuteActivityFilter(configurator, filterType, context, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseExecuteActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="ExecuteContext{TArguments}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration registration context</param>
        public static void UseExecuteActivityFilter<TFilter>(this IConsumePipeConfigurator configurator, IRegistrationContext context)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();

            messageTypeFilterConfigurator.Include(type =>
                typeof(IFilter<>).MakeGenericType(typeof(ExecuteContext<>).MakeGenericType(type)).IsAssignableFrom(filterType));

            var observer = new ScopedExecuteActivityPipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
        {
            UseCompensateActivityFilter(configurator, filterType, context, null);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filterType">Filter type</param>
        /// <param name="context">Configuration registration context</param>
        /// <param name="configureMessageTypeFilter">Message type to which apply the filter</param>
        public static void UseCompensateActivityFilter(this IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context,
            Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!filterType.IsGenericType || !filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();
            configureMessageTypeFilter?.Invoke(messageTypeFilterConfigurator);

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }

        /// <summary>
        /// Use scoped filter for <see cref="CompensateContext{T}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration registration context</param>
        public static void UseCompensateActivityFilter<TFilter>(this IConsumePipeConfigurator configurator, IRegistrationContext context)
            where TFilter : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var filterType = typeof(TFilter);
            var messageTypeFilterConfigurator = new MessageTypeFilterConfigurator();

            messageTypeFilterConfigurator.Include(type =>
                typeof(IFilter<>).MakeGenericType(typeof(CompensateContext<>).MakeGenericType(type)).IsAssignableFrom(filterType));

            var observer = new ScopedCompensateActivityPipeSpecificationObserver(filterType, context, messageTypeFilterConfigurator.Filter);
            configurator.ConnectActivityConfigurationObserver(observer);
        }
    }
}
