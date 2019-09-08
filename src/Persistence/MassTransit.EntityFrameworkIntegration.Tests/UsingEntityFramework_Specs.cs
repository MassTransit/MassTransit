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
namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using EntityFrameworkIntegration;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("EntityFramework")]
    public class When_using_EntityFramework :
        InMemoryTestFixture
    {
        SuperShopper _machine;
        readonly ISagaDbContextFactory<ShoppingChore> _sagaDbContextFactory;
        readonly Lazy<ISagaRepository<ShoppingChore>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            configurator.UseRetry(x =>
            {
                x.Handle<DbUpdateException>();
                x.Immediate(5);
            });

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFramework()
        {
            _sagaDbContextFactory = new DelegateSagaDbContextFactory<ShoppingChore>(
                () => new SagaDbContext<ShoppingChore, EntityFrameworkShoppingChoreMap>(SagaDbContextFactoryProvider.GetLocalDbConnectionString()));

            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() => EntityFrameworkSagaRepository<ShoppingChore>.CreatePessimistic(_sagaDbContextFactory));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var sagaInstance = dbContext.Set<ShoppingChore>().SingleOrDefault(x => x.CorrelationId == id);
                return sagaInstance;
            }
        }

        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);
            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new SodOff
            {
                CorrelationId = correlationId
            });

            sagaId = await ExtensionMethodsForSagas.ShouldNotContainSaga<ShoppingChore>(_repository.Value, correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
        }

        [Test, Explicit]
        public async Task Should_handle_the_big_load()
        {
            var tasks = new List<Task>();

            Guid[] sagaIds = new Guid[200];
            for (int i = 0; i < 200; i++)
            {
                Guid correlationId = Guid.NewGuid();

                tasks.Add(InputQueueSendEndpoint.Send(new GirlfriendYelling
                {
                    CorrelationId = correlationId
                }));

                sagaIds[i] = correlationId;
            }

            await Task.WhenAll(tasks);

            for (int i = 0; i < 200; i++)
            {
                Guid? sagaId = await _repository.Value.ShouldContainSaga(sagaIds[i], TestTimeout);
                Assert.IsTrue(sagaId.HasValue);
            }
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new GotHitByACar
            {
                CorrelationId = correlationId
            });

            sagaId = await ExtensionMethodsForSagas.ShouldContainSaga<ShoppingChore>(_repository.Value, x => x.CorrelationId == correlationId
                && x.CurrentState == _machine.Dead.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            ShoppingChore instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }


    class EntityFrameworkShoppingChoreMap :
        SagaClassMapping<ShoppingChore>
    {
        public EntityFrameworkShoppingChoreMap()
        {
            Property(x => x.CurrentState);
            Property(x => x.Everything);

            Property(x => x.Screwed);
        }
    }
}
