namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("EntityFramework")]
    [Category("Flaky")]
    public class When_using_EntityFrameworkConcurrencyFail :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_capture_all_events_many_sagas()
        {
            var tasks = new List<Task>();

            var sagaIds = new Guid[20];
            for (var i = 0; i < 20; i++)
            {
                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

                sagaIds[i] = correlationId;
            }

            for (var i = 0; i < 20; i++)
            {
                Guid? sagaId = await _repository.Value.ShouldContainSaga(sagaIds[i], TestTimeout);
                Assert.IsTrue(sagaId.HasValue);
            }

            for (var i = 0; i < 20; i++)
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
                Guid? sagaId = await _repository.Value.ShouldContainSagaInState(sid, _machine, _machine.Warmup, TestTimeout);

                Assert.IsTrue(sagaId.HasValue);
            }
        }

        [Test]
        public async Task Should_not_capture_all_events_single_saga()
        {
            var correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

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

            sagaId = await _repository.Value.ShouldContainSagaInState(correlationId, _machine, _machine.Warmup, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);
        }

        ChoirStateMachine _machine;
        readonly ISagaDbContextFactory<ChoirStateOptimistic> _sagaDbContextFactory;
        readonly Lazy<ISagaRepository<ChoirStateOptimistic>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStateMachine();

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFrameworkConcurrencyFail()
        {
            _sagaDbContextFactory = new DelegateSagaDbContextFactory<ChoirStateOptimistic>(() =>
                new ChoirStateOptimisticSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

            _repository = new Lazy<ISagaRepository<ChoirStateOptimistic>>(() =>
                EntityFrameworkSagaRepository<ChoirStateOptimistic>.CreateOptimistic(_sagaDbContextFactory));
        }

        async Task<ChoirStateOptimistic> GetSaga(Guid id)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                return await dbContext.Set<ChoirStateOptimistic>().SingleOrDefaultAsync(x => x.CorrelationId == id);
            }
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ConcurrentMessageLimit = 16;
        }
    }
}
