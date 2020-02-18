namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EntityFrameworkIntegration;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("EntityFramework")]
    public class When_using_EntityFrameworkConcurrencyPessimistic :
        InMemoryTestFixture
    {
        ChoirStatePessimisticMachine _machine;
        readonly ISagaDbContextFactory<ChoirStatePessimistic> _sagaDbContextFactory;
        readonly Lazy<ISagaRepository<ChoirStatePessimistic>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStatePessimisticMachine();

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFrameworkConcurrencyPessimistic()
        {
            _sagaDbContextFactory = new DelegateSagaDbContextFactory<ChoirStatePessimistic>(() =>
                new ChoirStatePessimisticSagaDbContext(SagaDbContextFactoryProvider.GetLocalDbConnectionString()));
            _repository = new Lazy<ISagaRepository<ChoirStatePessimistic>>(() =>
                 EntityFrameworkSagaRepository<ChoirStatePessimistic>.CreatePessimistic(_sagaDbContextFactory));
        }

        async Task<ChoirStatePessimistic> GetSaga(Guid id)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                return await dbContext.Set<ChoirStatePessimistic>().SingleOrDefaultAsync(x => x.CorrelationId == id);
            }
        }

        [Test]
        public async Task Should_capture_all_events_many_sagas()
        {
            var tasks = new List<Task>();

            Guid[] sagaIds = new Guid[20];
            for (int i = 0; i < 20; i++)
            {
                Guid correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehersalBegins {CorrelationId = correlationId});

                sagaIds[i] = correlationId;
            }

            for (int i = 0; i < 20; i++)
            {
                Guid? sagaId = await _repository.Value.ShouldContainSaga(sagaIds[i], TestTimeout);
                Assert.IsTrue(sagaId.HasValue);
            }

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(InputQueueSendEndpoint.Send(new Bass
                {
                    CorrelationId = sagaIds[i],
                    Name = "John"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Baritone
                {
                    CorrelationId = sagaIds[i],
                    Name = "Mark"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Tenor
                {
                    CorrelationId = sagaIds[i],
                    Name = "Anthony"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Countertenor
                {
                    CorrelationId = sagaIds[i],
                    Name = "Tom"
                }));
            }

            await Task.WhenAll(tasks);
            tasks.Clear();

            foreach (var sid in sagaIds)
            {
                var sagaId = await _repository.Value.ShouldContainSaga(x => x.CorrelationId == sid
                    && x.CurrentState == _machine.Harmony.Name, TestTimeout);

                Assert.IsTrue(sagaId.HasValue);
            }
        }

        [Test]
        public async Task Should_capture_all_events_single_saga()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new RehersalBegins {CorrelationId = correlationId});

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await Task.WhenAll(
                InputQueueSendEndpoint.Send(new Bass
                {
                    CorrelationId = correlationId,
                    Name = "John"
                }),
                InputQueueSendEndpoint.Send(new Baritone
                {
                    CorrelationId = correlationId,
                    Name = "Mark"
                }),
                InputQueueSendEndpoint.Send(new Tenor
                {
                    CorrelationId = correlationId,
                    Name = "Anthony"
                }),
                InputQueueSendEndpoint.Send(new Countertenor
                {
                    CorrelationId = correlationId,
                    Name = "Tom"
                })
            );

            sagaId = await _repository.Value.ShouldContainSaga(x => x.CorrelationId == correlationId
                && x.CurrentState == _machine.Harmony.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            ChoirStatePessimistic instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.CurrentState.Equals("Harmony"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }
}
