namespace MassTransit.DependencyInjection.Testing
{
    using System;
    using Configuration;
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;


    public class SagaContainerTestHarnessRegistration<TSaga> :
        ISagaRepositoryDecoratorRegistration<TSaga>
        where TSaga : class, ISaga
    {
        public SagaContainerTestHarnessRegistration(ITestHarness testHarness)
        {
            TestTimeout = testHarness.TestTimeout;

            Consumed = new ReceivedMessageList(testHarness.TestTimeout, testHarness.InactivityToken);
            Created = new SagaList<TSaga>(testHarness.TestTimeout, testHarness.InactivityToken);
            Sagas = new SagaList<TSaga>(testHarness.TestTimeout, testHarness.InactivityToken);
        }

        public TimeSpan TestTimeout { get; }

        public ReceivedMessageList Consumed { get; }
        public SagaList<TSaga> Created { get; }
        public SagaList<TSaga> Sagas { get; }

        public ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository)
        {
            return new TestSagaRepositoryDecorator<TSaga>(repository, Consumed, Created, Sagas);
        }
    }
}
