namespace MassTransit.EntityFrameworkIntegration.Tests
{
    namespace PreInsert
    {
        using System;
        using System.Collections.Generic;
        using System.Data.Entity;
        using System.Data.Entity.ModelConfiguration;
        using System.Threading.Tasks;
        using MassTransit.Saga;
        using NUnit.Framework;
        using Saga;
        using TestFramework;
        using Testing;


        public class Instance :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public DateTime CreateTimestamp { get; set; }
            public string Name { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
            }

            public Start(string name)
            {
                CorrelationId = NewId.NextGuid();
                Name = name;
                Timestamp = DateTime.UtcNow;
            }

            public Start(Guid correlationId, string name)
            {
                CorrelationId = correlationId;
                Name = name;
                Timestamp = DateTime.UtcNow;
            }

            public string Name { get; private set; }
            public DateTime Timestamp { get; private set; }
            public Guid CorrelationId { get; private set; }
        }


        class WarmUp :
            CorrelatedBy<Guid>
        {
            public WarmUp()
            {
            }

            public WarmUp(Guid correlationId, string name)
            {
                CorrelationId = correlationId;
                Name = name;
                Timestamp = DateTime.UtcNow;
            }

            public string Name { get; private set; }
            public DateTime Timestamp { get; private set; }
            public Guid CorrelationId { get; private set; }
        }


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }


    #region EntityFramework

        public class InstanceSagaDbContext : SagaDbContext
        {
            public InstanceSagaDbContext(string dbConnectionString)
                : base(dbConnectionString)
            {
            }

            protected override IEnumerable<ISagaClassMap> Configurations
            {
                get { yield return new EntityFrameworkInstanceMap(); }
            }


            class EntityFrameworkInstanceMap : SagaClassMap<Instance>
            {
                protected override void Configure(EntityTypeConfiguration<Instance> entity, DbModelBuilder modelBuilder)
                {
                    entity.Property(x => x.CurrentState);
                    entity.Property(x => x.Name);
                    entity.Property(x => x.CreateTimestamp);
                }
            }
        }


        [TestFixture]
        [Explicit]
        public class When_pre_inserting_the_state_machine_instance_using_ef :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_receive_the_published_message()
            {
                Task<ConsumeContext<StartupComplete>> messageReceived = await ConnectPublishHandler<StartupComplete>();

                var message = new Start("Joe");

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<StartupComplete> received = await messageReceived;

                Assert.AreEqual(message.CorrelationId, received.Message.TransactionId);

                Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

                Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

                Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

                Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, _machine.Running, TestTimeout);

                Assert.IsTrue(saga.HasValue);
            }

            public When_pre_inserting_the_state_machine_instance_using_ef()
            {
                ISagaDbContextFactory<Instance> sagaDbContextFactory = new DelegateSagaDbContextFactory<Instance>(
                    () => new InstanceSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

                _repository = EntityFrameworkSagaRepository<Instance>.CreatePessimistic(sagaDbContextFactory);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _machine = new TestStateMachine();

                configurator.StateMachineSaga(_machine, _repository);
            }

            TestStateMachine _machine;
            readonly ISagaRepository<Instance> _repository;


            class TestStateMachine :
                MassTransitStateMachine<Instance>
            {
                public TestStateMachine()
                {
                    InstanceState(x => x.CurrentState);

                    Event(() => Started, x =>
                    {
                        x.InsertOnInitial = true;
                        x.SetSagaFactory(context => new Instance
                        {
                            // the CorrelationId header is the value that should be used
                            CorrelationId = context.CorrelationId.Value,
                            CreateTimestamp = context.Message.Timestamp,
                            Name = context.Message.Name
                        });
                    });

                    Initially(
                        When(Started)
                            .Publish(context => new StartupComplete {TransactionId = context.Data.CorrelationId})
                            .TransitionTo(Running));
                }

                public State Running { get; private set; }
                public Event<Start> Started { get; private set; }
            }
        }


        [TestFixture]
        public class When_pre_inserting_in_an_invalid_state_using_ef :
            InMemoryTestFixture
        {
            [Test]
            [Explicit]
            public async Task Should_receive_the_published_message()
            {
                Task<ConsumeContext<StartupComplete>> messageReceived = await ConnectPublishHandler<StartupComplete>();

                var sagaId = NewId.NextGuid();

                var warmUpMessage = new WarmUp(sagaId, "Joe");

                await InputQueueSendEndpoint.Send(warmUpMessage);

                await _repository.ShouldContainSaga(sagaId, TestTimeout);

                var message = new Start(sagaId, "Joe");

                await Task.Delay(1000);

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<StartupComplete> received = await messageReceived;

                Assert.AreEqual(sagaId, received.Message.TransactionId);

                Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

                Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

                Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

                Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, _machine.Running, TestTimeout);

                Assert.IsTrue(saga.HasValue);
            }

            public When_pre_inserting_in_an_invalid_state_using_ef()
            {
                var sagaDbContextFactory =
                    new DelegateSagaDbContextFactory<Instance>(() => new InstanceSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

                _repository = EntityFrameworkSagaRepository<Instance>.CreatePessimistic(sagaDbContextFactory);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _machine = new TestStateMachine();

                configurator.UseMessageRetry(r => r.Immediate(5));
                configurator.StateMachineSaga(_machine, _repository);
            }

            TestStateMachine _machine;
            readonly ISagaRepository<Instance> _repository;


            class TestStateMachine :
                MassTransitStateMachine<Instance>
            {
                public TestStateMachine()
                {
                    InstanceState(x => x.CurrentState);

                    Event(() => WarmedUp, x =>
                    {
                        x.InsertOnInitial = true;
                        x.SetSagaFactory(context => new Instance
                        {
                            // the CorrelationId header is the value that should be used
                            CorrelationId = context.CorrelationId.Value,
                            CreateTimestamp = context.Message.Timestamp,
                            Name = context.Message.Name
                        });
                    });

                    Initially(
                        When(WarmedUp)
                            .Then(context =>
                            {
                            }),
                        When(Started)
                            .Publish(context => new StartupComplete {TransactionId = context.Data.CorrelationId})
                            .TransitionTo(Running));
                }

                public State Running { get; private set; }
                public Event<Start> Started { get; private set; }
                public Event<WarmUp> WarmedUp { get; private set; }
            }
        }

    #endregion EntityFramework
    }
}
