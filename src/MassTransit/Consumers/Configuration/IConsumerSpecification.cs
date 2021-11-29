namespace MassTransit.Configuration
{
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

        /// <summary>
        /// Apply any consumer-wide configurations to the message pipe, such as concurrency limit, etc.
        /// </summary>
        /// <param name="pipeConfigurator"></param>
        /// <typeparam name="T"></typeparam>
        void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
            where T : class;
    }
}
