namespace MassTransit.Azure.Table.Tests.SlowConcurrentSaga
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable.Saga;
    using DataAccess;
    using Events;
    using NUnit.Framework;
    using Shouldly;
    using Testing;


    public class SlowConcurrentSaga_Specs : AzureTableInMemoryTestFixture
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
            var activityMonitor = Bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(3000));

            var sagaId = NewId.NextGuid();
            var message = new Begin { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var slowMessage = new IncrementCounterSlowly { CorrelationId = sagaId };
            await Task.WhenAll(
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)),
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)));

            _sagaTestHarness.Consumed.Select<IncrementCounterSlowly>().Take(2).ToList();

            await activityMonitor.AwaitBusInactivity(TestTimeout);

            await _sagaRepository.Value.ShouldContainSaga(sagaId, s => s.Counter == 3, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 16;
        }
    }
}
