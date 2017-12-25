// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using Automatonymous;
    using EntityFrameworkIntegration;
    using EntityFrameworkIntegration.Saga;
    using GreenPipes;
    using NUnit.Framework;
    using Saga;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_using_EntityFrameworkConcurrencyOptimistic :
        InMemoryTestFixture
    {
        ChoirStateMachine _machine;
        readonly SagaDbContextFactory _sagaDbContextFactory;
        readonly Lazy<ISagaRepository<ChoirState>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStateMachine();

            configurator.UseRetry(x =>
            {
                x.Handle<DbUpdateConcurrencyException>();
                x.Immediate(5);
            });
            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFrameworkConcurrencyOptimistic()
        {
            _sagaDbContextFactory =
                () => new SagaDbContext<ChoirState, EntityFrameworkChoirStateMap>(SagaDbContextFactoryProvider.GetLocalDbConnectionString());
            _repository = new Lazy<ISagaRepository<ChoirState>>(() => new EntityFrameworkSagaRepository<ChoirState>(_sagaDbContextFactory, optimistic: true));
        }

        async Task<ChoirState> GetSaga(Guid id)
        {
            using (var dbContext = _sagaDbContextFactory())
            {
                return await dbContext.Set<ChoirState>().SingleOrDefaultAsync(x => x.CorrelationId == id);
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

                await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

                sagaIds[i] = correlationId;
            }

            for (int i = 0; i < 20; i++)
            {
                Guid? sagaId = await _repository.Value.ShouldContainSaga(sagaIds[i], TestTimeout);
                Assert.IsTrue(sagaId.HasValue);
            }

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(InputQueueSendEndpoint.Send(new Bass { CorrelationId = sagaIds[i], Name = "John" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Baritone { CorrelationId = sagaIds[i], Name = "Mark" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Tenor { CorrelationId = sagaIds[i], Name = "Anthony" }));
                tasks.Add(InputQueueSendEndpoint.Send(new Countertenor { CorrelationId = sagaIds[i], Name = "Tom" }));
            }

            await Task.WhenAll(tasks);
            tasks.Clear();

            foreach(var sid in sagaIds)
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

            await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await Task.WhenAll(
                InputQueueSendEndpoint.Send(new Bass { CorrelationId = correlationId, Name = "John" }),
                InputQueueSendEndpoint.Send(new Baritone { CorrelationId = correlationId, Name = "Mark" }),
                InputQueueSendEndpoint.Send(new Tenor { CorrelationId = correlationId, Name = "Anthony" }),
                InputQueueSendEndpoint.Send(new Countertenor { CorrelationId = correlationId, Name = "Tom" })
                );

            sagaId = await _repository.Value.ShouldContainSaga(x => x.CorrelationId == correlationId
                && x.CurrentState == _machine.Harmony.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            ChoirState instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.CurrentState.Equals("Harmony"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }


    class EntityFrameworkChoirStateMap :
        SagaClassMapping<ChoirState>
    {
        public EntityFrameworkChoirStateMap()
        {
            Property(x => x.RowVersion)
                .IsRowVersion();

            Property(x => x.CurrentState);
            Property(x => x.BassName);
            Property(x => x.BaritoneName);
            Property(x => x.TenorName);
            Property(x => x.CountertenorName);

            Property(x => x.Harmony);
        }
    }
}