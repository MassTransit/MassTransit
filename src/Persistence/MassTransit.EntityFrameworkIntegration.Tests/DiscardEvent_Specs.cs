// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using EntityFrameworkIntegration;
    using Mappings;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class StateMachineTest :
        InMemoryTestFixture
    {
        ISagaDbContextFactory _sagaDbContextFactory;

        [Test]
        public async Task Test_Discarded_Missing_Saga_Instance_Is_Not_Persisted()
        {
            // arrange
            var sagaId = Guid.NewGuid();

            // act
            await Bus.Publish<TriggerSecond>(new
            {
                CorrelationId = sagaId,
                TestName = "Test_Discarded_Missing_Saga_Instance_Is_Not_Persisted"
            });

            var wasDiscarded = await _discarded.Task;

            Assert.IsTrue(wasDiscarded);

            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var result = dbContext.Set<SimpleState>().FirstOrDefault(x => x.CorrelationId == sagaId);
                // THE PROBLEM : the missing instance is not discarded and is persisted to the repository
                // This test fails
                Assert.IsNull(result);
            }
        }

        [Test, Explicit]
        public async Task Test_Event_Persists_Saga_By_Default()
        {
            // arrange
            var sagaId = Guid.NewGuid();

            // act
            await Bus.Publish<TriggerFirst>(new
            {
                CorrelationId = sagaId,
                TestName = "Test_Event_Persists_Saga_By_Default"
            });

            await Task.Delay(10 * 1000).ConfigureAwait(false);

            // assert
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var result = dbContext.Set<SimpleState>().FirstOrDefault(x => x.CorrelationId == sagaId);
                Assert.IsNotNull(result);
            }
        }

        SimpleStateMachine _simpleStateMachine;
        Lazy<ISagaRepository<SimpleState>> _simpleStateRepository;
        TaskCompletionSource<bool> _discarded;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _discarded = GetTask<bool>();

            _simpleStateMachine = new SimpleStateMachine(x =>
            {
                _discarded.TrySetResult(true);
            });

            _sagaDbContextFactory = new DelegateSagaDbContextFactory(() =>
                new SimpleStateSagaDbContext(SagaDbContextFactoryProvider.GetLocalDbConnectionString()));

            _simpleStateRepository = new Lazy<ISagaRepository<SimpleState>>(() =>
                new EntityFrameworkSagaRepository<SimpleState>(_sagaDbContextFactory, System.Data.IsolationLevel.Serializable, new MsSqlLockStatements()));


            configurator.StateMachineSaga(_simpleStateMachine, _simpleStateRepository.Value);

            base.ConfigureInMemoryReceiveEndpoint(configurator);
        }
    }


    public class TriggerFirst : CorrelatedBy<Guid>
    {
        public string TestName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class TriggerSecond : CorrelatedBy<Guid>
    {
        public string TestName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SimpleState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string TestName { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SimpleStateSagaDbContext : SagaDbContext
    {
        class SimpleStateMap :
            SagaClassMap<SimpleState>
        {
            protected override void Configure(EntityTypeConfiguration<SimpleState> cfg, DbModelBuilder modelBuilder)
            {
                cfg.Property(x => x.CurrentState)
                    .HasMaxLength(64);

                cfg.Property(x => x.CorrelationId);
                cfg.Property(x => x.TestName);
                cfg.Property(x => x.PublishDate);
            }
        }

        public SimpleStateSagaDbContext(string getLocalDbConnectionString)
            : base(getLocalDbConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleStateMap(); }
        }
    }


    public sealed class SimpleStateMachine :
        MassTransitStateMachine<SimpleState>
    {
        public SimpleStateMachine(Action<string> executionMessage)
        {
            Event(() => FirstTrigger);
            Event(() => SecondTrigger,
                x => x.OnMissingInstance(e =>
                {
                    executionMessage("Missing Instance!!!");
                    return e.Discard();
                }));

            InstanceState(i => i.CurrentState);

            Initially(
                When(FirstTrigger)
                    .Then(DoFirstThing)
                    .TransitionTo(InProgress));

            DuringAny(
                When(SecondTrigger)
                    .Then(DoSecondThing)
                    .TransitionTo(Pending));
        }

        public Event<TriggerFirst> FirstTrigger { get; set; }
        public Event<TriggerSecond> SecondTrigger { get; set; }

        public State InProgress { get; set; }
        public State Pending { get; set; }

        void DoFirstThing(BehaviorContext<SimpleState, TriggerFirst> obj)
        {
            obj.Instance.TestName = obj.Data.TestName;
            obj.Instance.PublishDate = DateTime.Now;
        }

        void DoSecondThing(BehaviorContext<SimpleState, TriggerSecond> obj)
        {
            obj.Instance.TestName = obj.Data.TestName;
            obj.Instance.PublishDate = DateTime.Now;
        }
    }
}
