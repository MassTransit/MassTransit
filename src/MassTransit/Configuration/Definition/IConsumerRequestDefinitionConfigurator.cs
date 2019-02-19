namespace MassTransit.Definition
{
    public interface IConsumerRequestDefinitionConfigurator<TConsumer, TMessage> :
        IConsumerMessageDefinitionConfigurator<TConsumer, TMessage>
        where TConsumer : class, IConsumer
        where TMessage : class
    {
        /// <summary>
        /// Defines a message type which may be sent in response to the request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void Responds<T>()
            where T : class;
    }
}
