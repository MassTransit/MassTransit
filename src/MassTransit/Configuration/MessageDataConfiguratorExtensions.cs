namespace MassTransit
{
    using System;
    using GreenPipes;
    using MessageData;
    using Transformation.TransformConfigurators;


    public static class MessageDataConfiguratorExtensions
    {
        /// <summary>
        /// Enable the loading of message data for the any message type that includes a MessageData property.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        public static void UseMessageData(this IConsumePipeConfigurator configurator, IMessageDataRepository repository)
        {
            var observer = new MessageDataConfigurationObserver(configurator, repository);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configure"></param>
        public static void UseMessageData<T>(this IConsumePipeConfigurator configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformSpecification = new MessageDataTransformSpecification<T>(repository);

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
        public static void UseMessageData<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformSpecification = new MessageDataTransformSpecification<T>(repository);

            configure?.Invoke(transformSpecification);

            IPipeSpecification<ConsumeContext<T>> specification = transformSpecification;

            configurator.AddPipeSpecification(specification);
        }
    }
}
