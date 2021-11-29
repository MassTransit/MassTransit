namespace MassTransit.DependencyInjection.Registration
{
    using MassTransit.Testing;


    public interface IConsumerFactoryDecoratorRegistration<TConsumer>
        where TConsumer : class, IConsumer
    {
        ReceivedMessageList Consumed { get; }

        /// <summary>
        /// Decorate the container-based consumer factory, returning the consumer factory that should be
        /// used for receive endpoint registration
        /// </summary>
        /// <param name="consumerFactory"></param>
        /// <returns></returns>
        IConsumerFactory<TConsumer> DecorateConsumerFactory(IConsumerFactory<TConsumer> consumerFactory);
    }
}
