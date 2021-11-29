namespace MassTransit.Tests.SagaStateMachineTests
{
    namespace Request_Specs
    {
        using System;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Sending_a_request_from_a_state_machine :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_handle_the_response()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();

                var memberId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    memberId,
                    Name = "Frank"
                });

                ConsumeContext<MemberRegistered> registered = await handler;

                Guid? saga = await _repository.ShouldContainSagaInState(memberId, _machine, x => x.Registered, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                var sagaInstance = _repository[saga.Value].Instance;
                Assert.That(sagaInstance.Name, Is.EqualTo("Frank"));
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                base.ConfigureInMemoryBus(configurator);

                configurator.ReceiveEndpoint("service_queue", ConfigureServiceQueueEndpoint);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInMemoryReceiveEndpoint(configurator);

                _repository = new InMemorySagaRepository<TestState>();

                _machine = new TestStateMachine();

                configurator.UseInMemoryOutbox();

                configurator.StateMachineSaga(_machine, _repository);
            }

            protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                configurator.Handler<ValidateName>(async context =>
                {
                    await context.RespondAsync<NameValidated>(new { RequestName = context.Message.Name });
                });
            }
        }


        class TestState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public string Name { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public interface RegisterMember
        {
            Guid MemberId { get; }
            string Name { get; }
        }


        public interface MemberRegistered
        {
            string Name { get; }
        }


        public interface ValidateName
        {
            string Name { get; }
        }


        public interface NameValidated
        {
            string Name { get; }
            string RequestName { get; }
        }


        class TestStateMachine :
            MassTransitStateMachine<TestState>
        {
            public TestStateMachine()
            {
                Event(() => Register, x =>
                {
                    x.CorrelateById(p => p.Message.MemberId);
                });

                Request(() => ValidateName, cfg =>
                {
                    cfg.Timeout = TimeSpan.Zero;
                });

                Initially(When(Register)
                    .Then(context =>
                    {
                        context.Instance.Name = context.Data.Name;
                    })
                    .Request(ValidateName, x => x.Init<ValidateName>(new { x.Instance.Name }))
                    .TransitionTo(ValidateName.Pending));

                During(ValidateName.Pending,
                    When(ValidateName.Completed)
                        .ThenAsync(async context =>
                        {
                            context.Instance.Name = context.Data.Name;
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Registered),
                    When(ValidateName.Faulted)
                        .TransitionTo(NameValidationFaulted),
                    When(ValidateName.TimeoutExpired)
                        .TransitionTo(NameValidationTimeout));
            }

            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            // ReSharper disable MemberCanBePrivate.Global
            public Request<TestState, ValidateName, NameValidated> ValidateName { get; }

            public Event<RegisterMember> Register { get; }

            public State Registered { get; }

            public State NameValidationFaulted { get; }
            public State NameValidationTimeout { get; }
        }
    }
}
