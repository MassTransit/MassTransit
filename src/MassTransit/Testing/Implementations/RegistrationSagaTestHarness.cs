namespace MassTransit.Testing.Implementations
{
    using Configuration;


    public class RegistrationSagaTestHarness<TSaga> :
        BaseSagaTestHarness<TSaga>,
        ISagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        public RegistrationSagaTestHarness(ISagaRepositoryDecoratorRegistration<TSaga> registration, ISagaRepository<TSaga> repository)
            : base(repository, registration.TestTimeout)
        {
            Consumed = registration.Consumed;
            Created = registration.Created;
            Sagas = registration.Sagas;
        }

        public IReceivedMessageList Consumed { get; }

        public ISagaList<TSaga> Sagas { get; }

        public ISagaList<TSaga> Created { get; }
    }
}
