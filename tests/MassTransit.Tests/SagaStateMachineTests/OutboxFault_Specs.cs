namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_send_faults_in_the_outbox
    {
        [Test]
        public async Task Should_handle_the_redelivery_of_a_scheduled_message()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

                    x.AddConfigureEndpointsCallback((context, name, cfg) =>
                    {
                        cfg.UseSendFilter(typeof(SomeSendFilter<>), context);
                    });

                    x.AddHandler((ConsumeContext<SomeRequest> context) => context.RespondAsync(new SomeResponse
                    {
                        Status = context.Message.Count >= 5 ? "Finished" : "Running"
                    }));

                    x.AddSagaStateMachine<SomeStateMachine, SomeInstance, SomeSagaDefinition>();
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            var id = NewId.NextGuid();

            await harness.Bus.Publish(new InitialEvent { CorrelationId = id });

            Assert.That(await harness.Published.Any<InstanceCompleted>(x => x.Context.Message.CorrelationId == id));

            await harness.Stop();
        }


        class SomeInstance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid? TokenId { get; set; }
            public int Count { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class SomeSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                if (context.Message is SomeInstanceEvent instanceEvent && instanceEvent.Count == 2)
                    context.Serializer = new DeathComesMeSerializer();

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }


            class DeathComesMeSerializer :
                IMessageSerializer
            {
                public ContentType ContentType => new ContentType("application/death");

                public MessageBody GetMessageBody<T1>(SendContext<T1> context)
                    where T1 : class
                {
                    throw new InvalidOperationException("In this scenario, death becomes me.");
                }
            }
        }


        class SomeStateMachine :
            MassTransitStateMachine<SomeInstance>
        {
            public SomeStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Request(() => HandlerRequest, x =>
                {
                    x.Timeout = TimeSpan.Zero;
                });

                Schedule(() => ScheduleEvent, x => x.TokenId, x =>
                {
                    x.Delay = TimeSpan.FromSeconds(1);
                    x.Received = r =>
                    {
                        r.CorrelateById(m => m.Message.CorrelationId);
                        r.ConfigureConsumeTopology = false;
                    };
                });

                Initially(
                    When(InitialEventReceived)
                        .Then(context => LogContext.Debug?.Log("Initial event, scheduling instance event"))
                        .Schedule(ScheduleEvent, context => new SomeInstanceEvent
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            Count = context.Saga.Count++
                        })
                        .TransitionTo(Running));

                During(Running,
                    When(ScheduleEvent.Received)
                        .Then(context => LogContext.Debug?.Log("Sending request"))
                        .Schedule(ScheduleEvent, context => new SomeInstanceEvent
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            Count = context.Saga.Count++
                        })
                        .Request(HandlerRequest, context => new SomeRequest { Count = context.Saga.Count })
                        .TransitionTo(Checking)
                );

                During(Checking,
                    When(ScheduleEvent.Received)
                        .Then(context => LogContext.Debug?.Log("Scheduled event received while waiting for request"))
                        .Schedule(ScheduleEvent, context => new SomeInstanceEvent
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            Count = context.Saga.Count++
                        })
                        .Request(HandlerRequest, context => new SomeRequest { Count = context.Saga.Count })
                        .TransitionTo(Suspect)
                );

                During(Suspect,
                    When(ScheduleEvent.Received)
                        .Then(context => LogContext.Debug?.Log("Suspect, scheduled event, faulted time"))
                        .TransitionTo(Failed)
                        .Publish(context => new InstanceCompleted
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            Result = "Faulted"
                        })
                );

                During(Running, Checking, Suspect,
                    When(HandlerRequest.Completed)
                        .IfElse(context => context.Message.Status == "Running", running => running
                                .Then(context => LogContext.Debug?.Log("Response received, back to running"))
                                .TransitionTo(Running), otherwise => otherwise
                                .Then(context => LogContext.Debug?.Log("Response received, to completed"))
                                .Finalize()
                        )
                );

                WhenEnter(Final, x => x.Unschedule(ScheduleEvent)
                    .Publish(context => new InstanceCompleted
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Result = "Success"
                    })
                );
            }

            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            public Schedule<SomeInstance, SomeInstanceEvent> ScheduleEvent { get; }

            public Request<SomeInstance, SomeRequest, SomeResponse> HandlerRequest { get; }

            public Event<InitialEvent> InitialEventReceived { get; }

            public State Running { get; }
            public State Checking { get; }
            public State Suspect { get; }
            public State Failed { get; }
        }


        class SomeSagaDefinition :
            SagaDefinition<SomeInstance>
        {
            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<SomeInstance> sagaConfigurator,
                IRegistrationContext context)
            {
                endpointConfigurator.UseMessageScope(context);
                endpointConfigurator.UseMessageRetry(r => r.Immediate(5));
                endpointConfigurator.UseInMemoryOutbox(context);
            }
        }


        public class SomeRequest
        {
            public int Count { get; set; }
        }


        public class SomeResponse
        {
            public string Status { get; set; }
        }


        public class SomeInstanceEvent
        {
            public Guid CorrelationId { get; set; }
            public int Count { get; set; }
        }


        public class InstanceCompleted
        {
            public Guid CorrelationId { get; set; }
            public string Result { get; set; }
        }


        public class InitialEvent
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
