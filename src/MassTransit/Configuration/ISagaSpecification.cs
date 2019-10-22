namespace MassTransit
{
    using GreenPipes;
    using Saga;


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
    }
}
