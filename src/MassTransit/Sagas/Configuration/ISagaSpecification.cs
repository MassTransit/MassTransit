namespace MassTransit.Configuration
{
    /// <summary>
    /// A consumer specification, that can be modified
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaSpecification<TSaga> :
        ISagaConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
            where T : class;

        /// <summary>
        /// Apply any saga-wide configurations to the message pipe, such as concurrency limit, etc.
        /// </summary>
        /// <param name="pipeConfigurator"></param>
        /// <typeparam name="T"></typeparam>
        void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
            where T : class;
    }
}
