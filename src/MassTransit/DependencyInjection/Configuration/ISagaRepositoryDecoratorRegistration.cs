namespace MassTransit.Configuration
{
    using System;
    using Testing;
    using Testing.Implementations;


    public interface ISagaRepositoryDecoratorRegistration<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Decorate the container-based saga repository, returning the saga repository that should be
        /// used for receive endpoint registration
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository);

        TimeSpan TestTimeout { get; }
        ReceivedMessageList Consumed { get; }
        SagaList<TSaga> Created { get; }
        SagaList<TSaga> Sagas { get; }
    }
}
