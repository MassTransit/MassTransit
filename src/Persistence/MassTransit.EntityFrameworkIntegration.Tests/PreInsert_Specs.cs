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
    using System.Threading.Tasks;
    using Automatonymous;
    using EntityFrameworkIntegration;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    namespace PreInsert
    {
        using MassTransit.Saga;


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
        class EntityFrameworkInstanceMap : SagaClassMapping<Instance>
        {
            public EntityFrameworkInstanceMap()
            {
                Property(x => x.CurrentState);
                Property(x => x.Name);
                Property(x => x.CreateTimestamp);
            }
        }

        [TestFixture]
        public class When_pre_inserting_the_state_machine_instance_using_ef :
            InMemoryTestFixture
        {
            public When_pre_inserting_the_state_machine_instance_using_ef()
            {
                ISagaDbContextFactory<Instance> sagaDbContextFactory = new DelegateSagaDbContextFactory<Instance>(
                    () => new SagaDbContext<Instance, EntityFrameworkInstanceMap>(SagaDbContextFactoryProvider.GetLocalDbConnectionString()));

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
                            .Publish(context => new StartupComplete
                            {
                                TransactionId = context.Data.CorrelationId
                            })
                            .TransitionTo(Running));
                }

                public State Running { get; private set; }
                public Event<Start> Started { get; private set; }
            }


            [Test]
            public async Task Should_receive_the_published_message()
            {
                Task<ConsumeContext<StartupComplete>> messageReceived = ConnectPublishHandler<StartupComplete>();

                var message = new Start("Joe");

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<StartupComplete> received = await messageReceived;

                Assert.AreEqual(message.CorrelationId, received.Message.TransactionId);

                Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

                Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

                Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

                Guid? saga =
                    await ExtensionMethodsForSagas.ShouldContainSaga<Instance>(_repository, x => x.CorrelationId == message.CorrelationId && x.CurrentState == _machine.Running.Name, TestTimeout);

                Assert.IsTrue(saga.HasValue);
            }
        }

        [TestFixture]
        public class When_pre_inserting_in_an_invalid_state_using_ef :
            InMemoryTestFixture
        {
            public When_pre_inserting_in_an_invalid_state_using_ef()
            {
                ISagaDbContextFactory<Instance> sagaDbContextFactory = new DelegateSagaDbContextFactory<Instance>(
                    () => new SagaDbContext<Instance, EntityFrameworkInstanceMap>(SagaDbContextFactoryProvider.GetLocalDbConnectionString()));

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
                            .Publish(context => new StartupComplete
                            {
                                TransactionId = context.Data.CorrelationId
                            })
                            .TransitionTo(Running));
                }

                public State Running { get; private set; }
                public Event<Start> Started { get; private set; }
                public Event<WarmUp> WarmedUp { get; private set; }
            }


            [Test]
            public async Task Should_receive_the_published_message()
            {
                Task<ConsumeContext<StartupComplete>> messageReceived = ConnectPublishHandler<StartupComplete>();

                Guid sagaId = NewId.NextGuid();

                var warmUpMessage = new WarmUp(sagaId, "Joe");

                await InputQueueSendEndpoint.Send(warmUpMessage);

                await _repository.ShouldContainSaga(sagaId, TestTimeout);

                var message = new Start(sagaId, "Joe");

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<StartupComplete> received = await messageReceived;

                Assert.AreEqual(sagaId, received.Message.TransactionId);

                Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

                Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

                Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

                Guid? saga =
                    await ExtensionMethodsForSagas.ShouldContainSaga<Instance>(_repository, x => x.CorrelationId == message.CorrelationId && x.CurrentState == _machine.Running.Name, TestTimeout);

                Assert.IsTrue(saga.HasValue);
            }
        }
        #endregion EntityFramework

    }
}
