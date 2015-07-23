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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using EntityFrameworkIntegration;
    using EntityFrameworkIntegration.Saga;
    using Mehdime.Entity;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class When_using_EntityFramework :
        InMemoryTestFixture
    {
        SuperShopper _machine;
        readonly DbContextScopeFactory _dbContextScopeFactory;
        Lazy<ISagaRepository<ShoppingChore>> _repository;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFramework()
        {
            _dbContextScopeFactory = new DbContextScopeFactory(new SagaDbContextFactoryProvider());
            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() => new EntityFrameworkSagaRepository<ShoppingChore>(_dbContextScopeFactory));
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<DbContext>();

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

            sagaId = await _repository.Value.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
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

            sagaId = await _repository.Value.ShouldContainSaga(x => x.CorrelationId == correlationId
                && x.CurrentState == _machine.Dead.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            ShoppingChore instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
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


    /// <summary>
    ///     Why to exit the door to go shopping
    /// </summary>
    public class GirlfriendYelling
    {
        public Guid CorrelationId { get; set; }
    }


    public class GotHitByACar
    {
        public Guid CorrelationId { get; set; }
    }


    public class SodOff
    {
        public Guid CorrelationId { get; set; }
    }


    public class ShoppingChore :
        SagaStateMachineInstance
    {
        protected ShoppingChore()
        {
        }

        public ShoppingChore(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public int Everything { get; set; }
        public bool Screwed { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SuperShopper :
        MassTransitStateMachine<ShoppingChore>
    {
        public SuperShopper()
        {
            InstanceState(x => x.CurrentState);

            Event(() => ExitFrontDoor, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => GotHitByCar, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => JustSodOff, x => x.CorrelateById(context => context.Message.CorrelationId));

            CompositeEvent(() => EndOfTheWorld, x => x.Everything, CompositeEventOptions.IncludeInitial, ExitFrontDoor, GotHitByCar);

            Initially(
                When(ExitFrontDoor)
                    .Then(context => Console.Write("Leaving!"))
                    .TransitionTo(OnTheWayToTheStore));

            During(OnTheWayToTheStore,
                When(GotHitByCar)
                    .Then(context => Console.WriteLine("Ouch!!"))
                    .TransitionTo(Dead));

            DuringAny(
                When(EndOfTheWorld)
                    .Then(context => Console.WriteLine("Screwed!!"))
                    .Then(context => context.Instance.Screwed = true));

            DuringAny(
                When(JustSodOff)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
        public Event<GotHitByACar> GotHitByCar { get; private set; }
        public Event<SodOff> JustSodOff { get; private set; }
        public Event EndOfTheWorld { get; private set; }
        public State OnTheWayToTheStore { get; private set; }
        public State Dead { get; private set; }
    }
}