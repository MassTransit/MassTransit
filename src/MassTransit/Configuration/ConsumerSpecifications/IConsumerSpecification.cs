namespace MassTransit.ConsumerSpecifications
{
    using ConsumeConfigurators;
    using GreenPipes;


    /// <summary>
    /// A consumer specification, that can be modified
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    public interface IConsumerSpecification<TConsumer> :
        IConsumerConfigurator<TConsumer>,
        ISpecification
        where TConsumer : class
    {
        IConsumerMessageSpecification<TConsumer, T> GetMessageSpecification<T>()
            where T : class;
    }
}
