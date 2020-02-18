namespace MassTransit.ApplicationInsights.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Azure.ServiceBus.Core;
    using Context;
    using Courier;
    using Courier.Contracts;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;
    using RabbitMqTransport;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture, Explicit]
    public class Using_the_telemetry_client :
        InMemoryTestFixture
    {
        readonly DependencyTrackingTelemetryModule _module;
        TelemetryClient _telemetryClient;
        InMemorySagaRepository<OrderState> _repository;
        OrderStateMachine _machine;

        public Using_the_telemetry_client()
        {
            _module = new DependencyTrackingTelemetryModule();
            _module.IncludeDiagnosticSourceActivities.Add("MassTransit");
        }

        [OneTimeSetUp]
        public void Setup()
        {
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "";
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            _telemetryClient = new TelemetryClient(configuration);

            _module.Initialize(configuration);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            _module.Dispose();
            _telemetryClient.Flush();

            await Task.Delay(5000);
        }

        [Test]
        public async Task Should_report_from_diagnostic_source()
        {
            _telemetryClient.TrackTrace("Submitting Order");

            var submitted = SubscribeHandler<OrderObserved>();

            await Bus.Publish<SubmitOrder>(new {OrderId = InVar.Id}, x => x.ResponseAddress = Bus.Address);

            var context = await submitted;

            var saga = await _repository.ShouldContainSaga(x => x.CorrelationId == context.Message.OrderId && Equals(x.CurrentState, _machine.PendingApproval),
                TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<SubmitOrderConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("order-observer", e =>
            {
                e.Consumer<OrderSubmittedConsumer>();
            });

            configurator.ReceiveEndpoint("order-state", e =>
            {
                _machine = new OrderStateMachine();
                _repository = new InMemorySagaRepository<OrderState>();

                e.StateMachineSaga(_machine, _repository);
            });
        }
    }


    [TestFixture, Explicit]
    public class Using_the_telemetry_client_with_rabbit_mq :
        RabbitMqTestFixture
    {
        readonly DependencyTrackingTelemetryModule _module;
        TelemetryClient _telemetryClient;
        InMemorySagaRepository<OrderState> _repository;
        OrderStateMachine _machine;
        TelemetryConfiguration _configuration;

        public Using_the_telemetry_client_with_rabbit_mq()
        {
            _module = new DependencyTrackingTelemetryModule();
            _module.IncludeDiagnosticSourceActivities.Add("MassTransit");
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = TelemetryConfiguration.CreateDefault();
            _configuration.InstrumentationKey = "";
            _configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            _telemetryClient = new TelemetryClient(_configuration);

            _module.Initialize(_configuration);

            var loggerOptions = new ApplicationInsightsLoggerOptions();

            var applicationInsightsLoggerProvider = new ApplicationInsightsLoggerProvider(Options.Create(_configuration),
                Options.Create(loggerOptions));

            ILoggerFactory factory = new LoggerFactory();
            factory.AddProvider(applicationInsightsLoggerProvider);

            LogContext.ConfigureCurrentLogContext(factory);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            _module.Dispose();
            _telemetryClient.Flush();

            await Task.Delay(5000);

            _configuration.Dispose();
        }

        [Test]
        public async Task Should_report_from_diagnostic_source()
        {
            _telemetryClient.TrackTrace("Submitting Order");

            var submitted = SubscribeHandler<OrderObserved>();
            var awaitingDelivery = ConnectPublishHandler<OrderAwaitingDelivery>();

            await Bus.Publish<SubmitOrder>(new {OrderId = InVar.Id}, x => x.ResponseAddress = Bus.Address);

            var context = await submitted;

            var saga = await _repository.ShouldContainSaga(x => x.CorrelationId == context.Message.OrderId && Equals(x.CurrentState, _machine.PendingApproval),
                TestTimeout);

            Assert.IsTrue(saga.HasValue);

            var deliveryContext = await awaitingDelivery;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<SubmitOrderConsumer>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("order-observer", e =>
            {
                e.Consumer<OrderSubmittedConsumer>();
            });

            configurator.ReceiveEndpoint("order-state", e =>
            {
                _machine = new OrderStateMachine();
                _repository = new InMemorySagaRepository<OrderState>();

                e.StateMachineSaga(_machine, _repository);

                EndpointConvention.Map<OrderProcessed>(e.InputAddress);
            });

            configurator.ReceiveEndpoint("execute-process-order", e =>
            {
                e.ExecuteActivityHost<ProcessOrderActivity, ProcessOrderArguments>();

                EndpointConvention.Map<ProcessOrderArguments>(e.InputAddress);
            });
        }
    }


    [TestFixture, Explicit]
    public class Using_the_telemetry_client_with_service_bus :
        AzureServiceBusTestFixture
    {
        readonly DependencyTrackingTelemetryModule _module;
        TelemetryClient _telemetryClient;
        InMemorySagaRepository<OrderState> _repository;
        OrderStateMachine _machine;
        TelemetryConfiguration _configuration;

        public Using_the_telemetry_client_with_service_bus()
        {
            _module = new DependencyTrackingTelemetryModule();
            _module.IncludeDiagnosticSourceActivities.Add("MassTransit");
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = TelemetryConfiguration.CreateDefault();
            _configuration.InstrumentationKey = "";
            _configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            _telemetryClient = new TelemetryClient(_configuration);

            _module.Initialize(_configuration);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            _module.Dispose();
            _telemetryClient.Flush();

            await Task.Delay(5000);

            _configuration.Dispose();
        }

        [Test]
        public async Task Should_report_from_diagnostic_source()
        {
            _telemetryClient.TrackTrace("Submitting Order");

            var submitted = SubscribeHandler<OrderObserved>();

            await Bus.Publish<SubmitOrder>(new {OrderId = InVar.Id}, x => x.ResponseAddress = Bus.Address);

            var context = await submitted;

            var saga = await _repository.ShouldContainSaga(x => x.CorrelationId == context.Message.OrderId && Equals(x.CurrentState, _machine.PendingApproval),
                TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<SubmitOrderConsumer>();
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("order-observer", e =>
            {
                e.Consumer<OrderSubmittedConsumer>();
            });

            configurator.ReceiveEndpoint("order-state", e =>
            {
                _machine = new OrderStateMachine();
                _repository = new InMemorySagaRepository<OrderState>();

                e.StateMachineSaga(_machine, _repository);

                EndpointConvention.Map<OrderProcessed>(e.InputAddress);
            });

            configurator.ReceiveEndpoint("execute-process-order", e =>
            {
                e.ExecuteActivityHost<ProcessOrderActivity, ProcessOrderArguments>();

                EndpointConvention.Map<ProcessOrderArguments>(e.InputAddress);
            });
        }
    }


    class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            LogContext.Info?.Log("Submitting Order: {OrderId}", context.Message.OrderId);

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            if (!EndpointConvention.TryGetDestinationAddress<ProcessOrderArguments>(out var activityAddress))
                throw new ConfigurationException("No endpoint address for activity");

            builder.AddActivity("Process", activityAddress);

            if (!EndpointConvention.TryGetDestinationAddress<OrderProcessed>(out var eventAddress))
                throw new ConfigurationException("No endpoint address for activity");

            builder.AddSubscription(eventAddress, RoutingSlipEvents.Completed, endpoint =>
                endpoint.Send<OrderProcessed>(context.Message));

            context.Execute(builder.Build());

            return context.Publish<OrderSubmitted>(context.Message, x => x.ResponseAddress = context.ResponseAddress);
        }
    }


    class OrderSubmittedConsumer :
        IConsumer<OrderSubmitted>
    {
        public Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            return context.RespondAsync<OrderObserved>(context.Message);
        }
    }


    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }


    public interface OrderSubmitted
    {
        Guid OrderId { get; }
    }


    public interface OrderObserved
    {
        Guid OrderId { get; }
    }


    public interface OrderProcessed
    {
        Guid OrderId { get; }
    }

    public interface OrderAwaitingDelivery
    {
        Guid OrderId { get; }
    }


    public class OrderState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
    }


    public interface ProcessOrderArguments
    {
        Guid OrderId { get; }
    }


    public class ProcessOrderActivity :
        IExecuteActivity<ProcessOrderArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<ProcessOrderArguments> context)
        {
            return context.Completed();
        }
    }


    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => Submitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => Processed, x => x.CorrelateById(m => m.Message.OrderId));

            Initially(
                When(Submitted)
                    .TransitionTo(PendingApproval),
                When(Processed)
                    .PublishAsync(x => x.Init<OrderAwaitingDelivery>(x.Data))
                    .TransitionTo(AwaitingDelivery));

            During(PendingApproval,
                When(Processed)
                    .PublishAsync(x => x.Init<OrderAwaitingDelivery>(x.Data))
                    .TransitionTo(AwaitingDelivery));

            During(AwaitingDelivery,
                Ignore(Submitted));
        }

        public Event<OrderSubmitted> Submitted { get; set; }
        public Event<OrderProcessed> Processed { get; set; }

        public State PendingApproval { get; set; }
        public State AwaitingDelivery { get; set; }
    }
}
