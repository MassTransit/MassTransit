namespace MassTransit
{
    using System;
    using GreenPipes;
    using MessageData;
    using MessageData.Conventions;
    using Transformation.TransformConfigurators;


    public static class MessageDataConfiguratorExtensions
    {
        /// <summary>
        /// Enable the loading of message data for the any message type that includes a MessageData property.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        [Obsolete("UseMessageData(repository) should be configured once, on the bus only, and will be used for all receive endpoints")]
        public static void UseMessageData(this IConsumePipeConfigurator configurator, IMessageDataRepository repository)
        {
            var observer = new CourierMessageDataConfigurationObserver(configurator, repository, true);
        }

        /// <summary>
        /// Enable the loading of message data for the any message type that includes a MessageData property.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        public static void UseMessageData(this IBusFactoryConfigurator configurator, IMessageDataRepository repository)
        {
            if (configurator.ConsumeTopology.TryAddConvention(new MessageDataConsumeTopologyConvention(repository))
                && configurator.SendTopology.TryAddConvention(new MessageDataSendTopologyConvention(repository)))
            {
                // Courier does not use ConsumeContext, so it needs to be special
                var observer = new CourierMessageDataConfigurationObserver(configurator, repository, false);
            }
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configure"></param>
        [Obsolete("UseMessageData(repository) should be configured once, on the bus only, and will be used for all receive endpoints")]
        public static void UseMessageData<T>(this IConsumePipeConfigurator configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformSpecification = new GetMessageDataTransformSpecification<T>(repository);

            configure?.Invoke(transformSpecification);

            IPipeSpecification<ConsumeContext<T>> specification = transformSpecification;

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configure"></param>
        [Obsolete("UseMessageData(repository) should be configured once, on the bus only, and will be used for all receive endpoints")]
        public static void UseMessageData<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformSpecification = new GetMessageDataTransformSpecification<T>(repository);

            configure?.Invoke(transformSpecification);

            IPipeSpecification<ConsumeContext<T>> specification = transformSpecification;

            configurator.AddPipeSpecification(specification);
        }
    }
}
