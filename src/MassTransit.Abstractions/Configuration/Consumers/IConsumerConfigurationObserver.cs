namespace MassTransit
{
    using System.ComponentModel;


    public interface IConsumerConfigurationObserver
    {
        /// <summary>
        /// Called when a consumer is configured
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class;

        /// <summary>
        /// Called when a consumer/message combination is configured
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class;
    }
}
