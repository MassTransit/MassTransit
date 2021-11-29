namespace MassTransit.Testing
{
    using Implementations;


    public class SagaTestHarness<TSaga> :
        BaseSagaTestHarness<TSaga>,
        ISagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        readonly ReceivedMessageList _consumed;
        readonly SagaList<TSaga> _created;
        readonly SagaList<TSaga> _sagas;

        public SagaTestHarness(BusTestHarness testHarness, ISagaRepository<TSaga> repository, string queueName)
            : base(repository, testHarness.TestTimeout)
        {
            _consumed = new ReceivedMessageList(testHarness.TestTimeout, testHarness.InactivityToken);
            _created = new SagaList<TSaga>(testHarness.TestTimeout, testHarness.InactivityToken);
            _sagas = new SagaList<TSaga>(testHarness.TestTimeout, testHarness.InactivityToken);

            TestRepository = new TestSagaRepositoryDecorator<TSaga>(repository, _consumed, _created, _sagas);

            if (string.IsNullOrWhiteSpace(queueName))
                testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
            else
                testHarness.OnConfigureBus += configurator => ConfigureNamedReceiveEndpoint(configurator, queueName);
        }

        protected TestSagaRepositoryDecorator<TSaga> TestRepository { get; }

        public IReceivedMessageList Consumed => _consumed;
        public ISagaList<TSaga> Sagas => _sagas;
        public ISagaList<TSaga> Created => _created;

        protected virtual void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(TestRepository);
        }

        protected virtual void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                x.Saga(TestRepository);
            });
        }
    }
}
