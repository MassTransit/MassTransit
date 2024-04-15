namespace MassTransit.Tests.SagaStateMachineTests
{
    namespace CompositeEventOnRequestResponses
    {
        using System;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using NUnit.Framework;
        using SagaStateMachine;
        using TestFramework;
        using Visualizer;


        [TestFixture]
        public class CompositeEventOnRequestResponsesTests : InMemoryTestFixture
        {
            [Test]
            public async Task Both_faulted()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();
                var memberId = NewId.NextGuid();
                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    memberId,
                    Name = "Bruce",
                    Surname = "Wayne"
                });
                ConsumeContext<MemberRegistered> registered = await handler;
                Guid? saga = await _repository.ShouldContainSagaInState(memberId, _machine, x => x.Rejected, TestTimeout);
                Assert.That(saga.HasValue, Is.True);
                var sagaInstance = _repository[saga.Value].Instance;
                Assert.Multiple(() =>
                {
                    Assert.That(sagaInstance.Result, Is.EqualTo("Invalid ID"));
                    Assert.That(sagaInstance.Name, Is.EqualTo("REJECTED!"));
                    Assert.That(sagaInstance.Surname, Is.EqualTo("REJECTED!"));
                });
            }

            [Test]
            public async Task Both_success()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();
                var memberId = NewId.NextGuid();
                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    memberId,
                    Name = "Frank",
                    Surname = "Castle"
                });
                ConsumeContext<MemberRegistered> registered = await handler;
                Guid? saga = await _repository.ShouldContainSagaInState(memberId, _machine, x => x.Registered, TestInactivityTimeout);
                Assert.That(saga.HasValue, Is.True);
                var sagaInstance = _repository[saga.Value].Instance;
                Assert.Multiple(() =>
                {
                    Assert.That(sagaInstance.Result, Is.EqualTo("Success"));
                    Assert.That(sagaInstance.Name, Is.EqualTo("Frank"));
                    Assert.That(sagaInstance.Surname, Is.EqualTo("Castle"));
                });
            }

            [Test]
            public async Task Name_faulted()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();
                var memberId = NewId.NextGuid();
                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    memberId,
                    Name = "Clark",
                    Surname = "Kent"
                });
                ConsumeContext<MemberRegistered> registered = await handler;
                Guid? saga = await _repository.ShouldContainSagaInState(memberId, _machine, x => x.Rejected, TestTimeout);
                Assert.That(saga.HasValue, Is.True);
                var sagaInstance = _repository[saga.Value].Instance;
                Assert.Multiple(() =>
                {
                    Assert.That(sagaInstance.Result, Is.EqualTo("Invalid Name"));
                    Assert.That(sagaInstance.Name, Is.EqualTo("REJECTED!"));
                    Assert.That(sagaInstance.Surname, Is.EqualTo("Kent"));
                });
            }

            [Test]
            public void Should_the_graph()
            {
                var dotsAssigned = new StateMachineGraphvizGenerator(new TestStateMachine().GetGraph()).CreateDotFile();

                Console.WriteLine(dotsAssigned);
            }

            [Test]
            public async Task Surname_faulted()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();
                var memberId = NewId.NextGuid();
                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    memberId,
                    Name = "Peter",
                    Surname = "Parker"
                });
                ConsumeContext<MemberRegistered> registered = await handler;
                Guid? saga = await _repository.ShouldContainSagaInState(memberId, _machine, x => x.Rejected, TestTimeout);
                Assert.That(saga.HasValue, Is.True);
                var sagaInstance = _repository[saga.Value].Instance;
                Assert.Multiple(() =>
                {
                    Assert.That(sagaInstance.Result, Is.EqualTo("Invalid Surname"));
                    Assert.That(sagaInstance.Name, Is.EqualTo("Peter"));
                    Assert.That(sagaInstance.Surname, Is.EqualTo("REJECTED!"));
                });
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
                configurator.UseInMemoryOutbox();
                configurator.UsePublishMessageScheduler();

                _repository = new InMemorySagaRepository<TestState>();
                _machine = new TestStateMachine();
                configurator.StateMachineSaga(_machine, _repository);
                EndpointConvention.Map<NameValidated>(configurator.InputAddress);
                EndpointConvention.Map<SurnameValidated>(configurator.InputAddress);
            }

            protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                configurator.Handler<ValidateName>(async context =>
                {
                    if (context.Message.Name == "Bruce" || context.Message.Name == "Clark")
                        throw new Exception("I do not like " + context.Message.Name);
                    await context.RespondAsync<NameValidated>(new { context.Message.Name });
                });
                configurator.Handler<ValidateSurname>(async context =>
                {
                    if (context.Message.Surname == "Wayne" || context.Message.Surname == "Parker")
                        throw new Exception("I do not like " + context.Message.Surname);
                    await context.RespondAsync<SurnameValidated>(new { context.Message.Surname });
                });
            }
        }


        class TestState : SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public string Name { get; set; }
            public string Surname { get; set; }
            public string Result { get; set; }
            public int CompositeStatus { get; set; }
            public int CompositeStatusBoth { get; set; }
            public int CompositeStatusFirst { get; set; }
            public int CompositeStatusSecond { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public interface RegisterMember
        {
            Guid MemberId { get; }
            string Name { get; }
            string Surname { get; }
        }


        public interface MemberRegistered
        {
            string Name { get; }
            string Surname { get; }
        }


        public interface ValidateName
        {
            string Name { get; }
        }


        public interface ValidateSurname
        {
            string Surname { get; }
        }


        public interface NameValidated
        {
            string Name { get; }
        }


        public interface SurnameValidated
        {
            string Surname { get; }
        }


        class TestStateMachine : MassTransitStateMachine<TestState>
        {
            public TestStateMachine()
            {
                Event(() => Register, x => x.CorrelateById(p => p.Message.MemberId));

                Request(() => ValidateName, cfg => cfg.Timeout = TimeSpan.FromSeconds(0));
                Request(() => ValidateSurname, cfg => cfg.Timeout = TimeSpan.FromSeconds(0));

                CompositeEvent(() => NameSurnameSuccess, x => x.CompositeStatus, ValidateName.Completed, ValidateSurname.Completed);
                CompositeEvent(() => InvalidId, x => x.CompositeStatusBoth, ValidateName.Faulted, ValidateSurname.Faulted);
                CompositeEvent(() => InvalidName, x => x.CompositeStatusFirst, ValidateName.Faulted, ValidateSurname.Completed);
                CompositeEvent(() => InvalidSurname, x => x.CompositeStatusSecond, ValidateName.Completed, ValidateSurname.Faulted);

                Initially(When(Register)
                    .Then(context =>
                    {
                        context.Instance.Name = context.Data.Name;
                        context.Instance.Surname = context.Data.Surname;
                    })
                    .Request(ValidateName, x => x.Init<ValidateName>(new { x.Instance.Name }))
                    .Request(ValidateSurname, x => x.Init<ValidateSurname>(new { x.Instance.Surname }))
                    .TransitionTo(Registering));

                DuringAny(
                    When(ValidateName.Completed)
                        .Then(context =>
                        {
                            TestContext.Out.WriteLine("ValidateName Completed");
                            context.Instance.Name = context.Data.Name;
                        }),
                    When(ValidateSurname.Completed)
                        .Then(context =>
                        {
                            TestContext.Out.WriteLine("ValidateSurname Completed");
                            context.Instance.Surname = context.Data.Surname;
                        }),
                    When(ValidateName.Faulted)
                        .Then(context =>
                        {
                            context.Instance.Name = "REJECTED!";
                        }),
                    When(ValidateSurname.Faulted)
                        .Then(context =>
                        {
                            context.Instance.Surname = "REJECTED!";
                        })
                );


                DuringAny(
                    When(NameSurnameSuccess)
                        .Then(context =>
                        {
                            TestContext.Out.WriteLine("Completed with Success");
                            context.Instance.Result = "Success";
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Registered)
                    ,

                    //--
                    When(InvalidId)
                        .Then(context =>
                        {
                            context.Instance.Result = "Invalid ID";
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Rejected),
                    When(InvalidName)
                        .Then(context =>
                        {
                            context.Instance.Result = "Invalid Name";
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Rejected),
                    When(InvalidSurname)
                        .Then(context =>
                        {
                            context.Instance.Result = "Invalid Surname";
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Rejected)
                );
            }

            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            // ReSharper disable MemberCanBePrivate.Global
            public Request<TestState, ValidateName, NameValidated> ValidateName { get; }
            public Request<TestState, ValidateSurname, SurnameValidated> ValidateSurname { get; }

            public Event<RegisterMember> Register { get; }
            public Event NameSurnameSuccess { get; private set; }
            public Event InvalidId { get; private set; }
            public Event InvalidName { get; private set; }
            public Event InvalidSurname { get; private set; }

            public State Registered { get; }
            public State Registering { get; }
            public State Rejected { get; }
        }
    }
}
