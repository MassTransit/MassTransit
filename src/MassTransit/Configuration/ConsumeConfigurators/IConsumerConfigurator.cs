namespace MassTransit.ConsumeConfigurators
{
    using System;
    using GreenPipes;


    public interface IConsumerConfigurator :
        IConsumeConfigurator,
        IConsumerConfigurationObserverConnector
    {
    }


    public interface IConsumerConfigurator<TConsumer> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer>>,
        IConsumerConfigurationObserverConnector,
        IConsumeConfigurator
        where TConsumer : class
    {
        /// <summary>
        /// Add middleware to the message pipeline, which is invoked prior to the consumer factory.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class;

        /// <summary>
        /// Add middleware to the consumer pipeline, for the specified message type, which is invoked
        /// after the consumer factory.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
            where T : class;

        /// <summary>
        /// Configure an options class, associated with the consumer, which may be used to configure
        /// aspects of the consumer.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The options type</typeparam>
        /// <returns></returns>
        T Options<T>(Action<T> configure = null)
            where T : IOptions, new();
    }
}
