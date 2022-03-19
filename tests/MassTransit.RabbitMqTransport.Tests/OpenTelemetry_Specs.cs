namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using HarnessContracts;
    using Initializers;
    using MassTransit.Testing;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using OpenTelemetry;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;
    using TestFramework.Courier;


    [TestFixture]
    [Explicit]
    public class OpenTelemetry_Specs
    {
        [Test]
        public async Task Should_report_telemetry_to_jaeger()
        {
            using var tracerProvider = CreateTraceProvider("order-api");

            var services = new ServiceCollection();

            await using var provider = services
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MonitoredSubmitOrderConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<SubmitOrder> client = harness.GetRequestClient<SubmitOrder>();

            await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });

            Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

            Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());
        }

        [Test]
        public async Task Should_report_telemetry_to_jaeger_for_routing_slip()
        {
            using var tracerProvider = CreateTraceProvider("routing-api");

            var services = new ServiceCollection();

            await using var provider = services
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<RoutingSlipEventConsumer>();
                    x.AddConsumer<RequestActivityProxy>();
                    x.AddConsumer<ResponseActivityProxy, ResponseActivityProxyDefinition>();

                    x.AddActivity<TestActivity, TestArguments, TestLog>();
                    x.AddActivity<SecondTestActivity, TestArguments, TestLog>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<SubmitActivity> client = harness.GetRequestClient<SubmitActivity>();

            Response<ActivityCompleted> response = await client.GetResponse<ActivityCompleted>(new { Value = "Hello" });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World!"));
            Assert.That(response.Message.Variable, Is.EqualTo("Knife"));
        }

        [Test]
        public async Task Should_report_telemetry_to_jaeger_from_mediator()
        {
            using var tracerProvider = CreateTraceProvider("mediator");

            var services = new ServiceCollection();

            await using var provider = services
                .AddMediator(x =>
                {
                    x.AddConsumer<MonitoredSubmitOrderConsumer>();
                    x.AddRequestClient<SubmitOrder>();
                })
                .BuildServiceProvider(true);

            var mediator = provider.GetRequiredService<IMediator>();

            using var scope = provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<SubmitOrder>>();

            await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });
        }

        [Test]
        public async Task Should_support_the_saga_harness()
        {
            using var tracerProvider = CreateTraceProvider("saga-api");

            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<SagaStartedConsumer>();

                    x.AddSagaStateMachine<TestStateMachine, Instance>()
                        .InMemoryRepository();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var sagaId = Guid.NewGuid();

            await harness.Bus.Publish(new Start { CorrelationId = sagaId });

            Assert.IsTrue(await harness.Consumed.Any<Start>(), "Message not received");

            var sagaHarness = provider.GetRequiredService<ISagaStateMachineTestHarness<TestStateMachine, Instance>>();

            Assert.That(await sagaHarness.Consumed.Any<Start>());

            Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == sagaId));

            var machine = provider.GetRequiredService<TestStateMachine>();

            var instance = sagaHarness.Created.ContainsInState(sagaId, machine, machine.Running);
            Assert.IsNotNull(instance, "Saga instance not found");

            Assert.IsTrue(await harness.Published.Any<Started>(), "Event not published");
        }

        static TracerProvider CreateTraceProvider(string serviceName)
        {
            return Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddSource("MassTransit")
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = "localhost";
                    o.AgentPort = 6831;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<System.Diagnostics.Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                })
                .Build();
        }


        class RoutingSlipEventConsumer :
            IConsumer<RoutingSlipActivityCompleted>
        {
            public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
            {
                return Task.CompletedTask;
            }
        }


        class RequestActivityProxy :
            RoutingSlipRequestProxy<SubmitActivity>
        {
            readonly Uri _secondTestActivityExecuteAddress;
            readonly Uri _testActivityExecuteAddress;

            public RequestActivityProxy(IEndpointNameFormatter endpointNameFormatter)
            {
                _testActivityExecuteAddress = new Uri($"exchange:{endpointNameFormatter.ExecuteActivity<TestActivity, TestArguments>()}");
                _secondTestActivityExecuteAddress = new Uri($"exchange:{endpointNameFormatter.ExecuteActivity<SecondTestActivity, TestArguments>()}");
            }

            protected override async Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<SubmitActivity> request)
            {
                builder.AddActivity("test", _testActivityExecuteAddress, new { request.Message.Value });

                builder.AddActivity("second-test", _secondTestActivityExecuteAddress);

                builder.AddVariable("Variable", "Knife");
                builder.AddVariable("Nothing", null);
                builder.AddVariable("ToBeRemoved", "Existing");
            }
        }


        class ResponseActivityProxy :
            RoutingSlipResponseProxy<SubmitActivity, ActivityCompleted>
        {
            protected override async Task<ActivityCompleted> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, SubmitActivity request)
            {
                var (message, _) = await MessageInitializerCache<ActivityCompleted>.InitializeMessage(new
                {
                    Value = context.GetVariable<string>("Value"),
                    Variable = context.GetVariable<string>("Variable")
                });

                return message;
            }
        }


        class ResponseActivityProxyDefinition :
            ConsumerDefinition<ResponseActivityProxy>
        {
            public ResponseActivityProxyDefinition(IEndpointNameFormatter endpointNameFormatter)
            {
                // must be on the same endpoint, to get responses
                EndpointName = endpointNameFormatter.Consumer<RequestActivityProxy>();
            }
        }


        class MonitoredSubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => StartReceived);

                Initially(
                    When(StartReceived)
                        .Activity(x => x.OfType<StartupActivity>())
                        .TransitionTo(Running));
            } // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public Event<Start> StartReceived { get; }
        }


        class StartupActivity :
            IStateMachineActivity<Instance, Start>
        {
            readonly IPublishEndpoint _publishEndpoint;

            public StartupActivity(IPublishEndpoint publishEndpoint)
            {
                _publishEndpoint = publishEndpoint;
            }

            public void Probe(ProbeContext context)
            {
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<Instance, Start> context, IBehavior<Instance, Start> next)
            {
                await _publishEndpoint.Publish(new Started { CorrelationId = context.Instance.CorrelationId });

                await next.Execute(context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<Instance, Start, TException> context, IBehavior<Instance, Start> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }
        }


        class SagaStartedConsumer :
            IConsumer<Started>
        {
            public Task Consume(ConsumeContext<Started> context)
            {
                return Task.CompletedTask;
            }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Started :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
