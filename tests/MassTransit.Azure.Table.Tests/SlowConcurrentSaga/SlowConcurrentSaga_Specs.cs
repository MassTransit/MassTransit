namespace MassTransit.Azure.Table.Tests.SlowConcurrentSaga
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable.Saga;
    using DataAccess;
    using Events;
    using NUnit.Framework;
    using Testing;


    public class SlowConcurrentSaga_Specs :
        AzureTableInMemoryTestFixture
    {
        readonly Lazy<ISagaRepository<SlowConcurrentSaga>> _sagaRepository;
        readonly ISagaStateMachineTestHarness<SlowConcurrentSagaStateMachine, SlowConcurrentSaga> _sagaTestHarness;

        public SlowConcurrentSaga_Specs()
        {
            _sagaRepository = new Lazy<ISagaRepository<SlowConcurrentSaga>>(() =>
                AzureTableSagaRepository<SlowConcurrentSaga>.Create(() => TestCloudTable));

            _sagaTestHarness = BusTestHarness.StateMachineSaga(new SlowConcurrentSagaStateMachine(), _sagaRepository.Value);
        }

        [Test]
        public async Task Two_Initiating_Messages_Deadlock_Results_In_One_Instance()
        {
            var sagaId = NewId.NextGuid();
            var message = new Begin { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestInactivityTimeout);

            Assert.That(foundId, Is.Not.Null);

            var slowMessage = new IncrementCounterSlowly { CorrelationId = sagaId };
            await Task.WhenAll(
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)),
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)));

            _ = _sagaTestHarness.Consumed.Select<IncrementCounterSlowly>().Take(2).ToList();

            await InactivityTask;

            Assert.That(await _sagaRepository.Value.ShouldContainSaga(sagaId, s => s.Counter == 3, TestInactivityTimeout), Is.Not.Null);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 16;

            configurator.UseMessageRetry(x => x.Interval(5, 100));
        }
    }
}
