namespace MassTransit
{
    using System;
    using Configuration;
    using Consumer;
    using Internals;


    public static class ConsumerExtensions
    {
        /// <summary>
        /// Connect a consumer to the receiving endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="configure">Optional, configure the consumer</param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory,
            Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (consumerFactory == null)
                throw new ArgumentNullException(nameof(consumerFactory));

            LogContext.Debug?.Log("Subscribing Consumer: {ConsumerType} (using supplied consumer factory)", TypeCache<TConsumer>.ShortName);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Connect a consumer to the bus instance's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="connector"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IConsumePipeConnector connector, IConsumerFactory<TConsumer> consumerFactory,
            params IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
            where TConsumer : class, IConsumer
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (consumerFactory == null)
                throw new ArgumentNullException(nameof(consumerFactory));

            LogContext.Debug?.Log("Connecting Consumer: {ConsumerType} (using supplied consumer factory)", TypeCache<TConsumer>.ShortName);

            IConsumerSpecification<TConsumer> specification = ConsumerConnectorCache<TConsumer>.Connector.CreateConsumerSpecification<TConsumer>();
            foreach (IPipeSpecification<ConsumerConsumeContext<TConsumer>> pipeSpecification in pipeSpecifications)
                specification.AddPipeSpecification(pipeSpecification);

            return ConsumerConnectorCache<TConsumer>.Connector.ConnectConsumer(connector, consumerFactory, specification);
        }

        /// <summary>
        /// Subscribes a consumer with a default constructor to the endpoint
        /// </summary>
        /// <typeparam name="TConsumer">The consumer type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer, new()
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            LogContext.Debug?.Log("Subscribing Consumer: {ConsumerType} (using default constructor)", TypeCache<TConsumer>.ShortName);

            var consumerFactory = new DefaultConstructorConsumerFactory<TConsumer>();

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Subscribe a consumer with a default constructor to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="connector"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IConsumePipeConnector connector,
            params IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
            where TConsumer : class, IConsumer, new()
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            LogContext.Debug?.Log("Connecting Consumer: {ConsumerType} (using default constructor)", TypeCache<TConsumer>.ShortName);

            return ConnectConsumer(connector, new DefaultConstructorConsumerFactory<TConsumer>(), pipeSpecifications);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, Func<TConsumer> consumerFactoryMethod,
            Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (consumerFactoryMethod == null)
                throw new ArgumentNullException(nameof(consumerFactoryMethod));

            LogContext.Debug?.Log("Subscribing Consumer: {ConsumerType} (using delegate consumer factory)", TypeCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(delegateConsumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Subscribe a consumer with a consumer factor method to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="connector"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IConsumePipeConnector connector, Func<TConsumer> consumerFactoryMethod,
            params IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
            where TConsumer : class, IConsumer
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (consumerFactoryMethod == null)
                throw new ArgumentNullException(nameof(consumerFactoryMethod));

            LogContext.Debug?.Log("Connecting Consumer: {ConsumerType} (using delegate consumer factory)", TypeCache<TConsumer>.ShortName);

            var consumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            return ConnectConsumer(connector, consumerFactory, pipeSpecifications);
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer (used by containers mostly)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerType"></param>
        /// <param name="consumerFactory"></param>
        /// <returns></returns>
        public static void Consumer(this IReceiveEndpointConfigurator configurator, Type consumerType, Func<Type, object> consumerFactory)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (consumerType == null)
                throw new ArgumentNullException(nameof(consumerType));
            if (consumerFactory == null)
                throw new ArgumentNullException(nameof(consumerFactory));

            LogContext.Debug?.Log("Subscribing Consumer: {ConsumerType} (by type, using object consumer factory)",
                TypeCache.GetShortName(consumerType));

            var configuratorType = typeof(UntypedConsumerConfigurator<>).MakeGenericType(consumerType);
            var consumerConfigurator = (IReceiveEndpointSpecification)Activator.CreateInstance(configuratorType, consumerFactory, configurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="consumerType"></param>
        /// <param name="objectFactory"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer(this IConsumePipeConnector connector, Type consumerType, Func<Type, object> objectFactory)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (consumerType == null)
                throw new ArgumentNullException(nameof(consumerType));
            if (objectFactory == null)
                throw new ArgumentNullException(nameof(objectFactory));
            if (!consumerType.HasInterface<IConsumer>())
                throw new ArgumentException("The consumer type must implement an IConsumer interface");

            LogContext.Debug?.Log("Connecting Consumer: {ConsumerType} (by type, using object consumer factory)", TypeCache.GetShortName(consumerType));

            return ConsumerConnectorCache.Connect(connector, consumerType, objectFactory);
        }
    }
}
