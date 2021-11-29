namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Courier;
    using Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;


    public class Using_the_outbox_with_the_request_client<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var client = GetRequestClient<InitialRequest>();

            await client.GetResponse<InitialResponse>(new { Value = "World" });
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddRequestClient<InitialRequest>();
            configurator.AddRequestClient<SubsequentRequest>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumer<InitialConsumer>(BusRegistrationContext);
        }


        class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IRequestClient<SubsequentRequest> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }
    }


    public class Using_the_outbox_with_a_routing_slip_request<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();

            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("SendRequestActivity", _executeAddress, new { });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        public Using_the_outbox_with_a_routing_slip_request()
        {
            RequestQueueName = "activity-request";
            RequestQueueAddress = new Uri($"queue:{RequestQueueName}");
        }

        Uri RequestQueueAddress { get; }
        string RequestQueueName { get; }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumer<ActivityRequestConsumer>();
            configurator.AddExecuteActivity<SendRequestActivity, SendRequestArguments>();

            configurator.AddConfigureEndpointsCallback((name, x) =>
            {
                x.UseInMemoryOutbox();

                if (name == KebabCaseEndpointNameFormatter.Instance.ExecuteActivity<SendRequestActivity, SendRequestArguments>())
                    _executeAddress = x.InputAddress;
            });

            configurator.AddRequestClient<ActivityRequest>(RequestQueueAddress);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();
        }


        class SendRequestActivity :
            IExecuteActivity<SendRequestArguments>
        {
            readonly IRequestClient<ActivityRequest> _client;

            public SendRequestActivity(IRequestClient<ActivityRequest> client)
            {
                _client = client;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<SendRequestArguments> context)
            {
                await _client.GetResponse<ActivityResponse>(new { Value = "Hello" });

                return context.Completed();
            }
        }


        public interface SendRequestArguments
        {
        }


        class ActivityRequestConsumer :
            IConsumer<ActivityRequest>
        {
            public Task Consume(ConsumeContext<ActivityRequest> context)
            {
                return context.RespondAsync<ActivityResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }


        public interface ActivityRequest
        {
            string Value { get; }
        }


        public interface ActivityResponse
        {
            string Value { get; }
        }
    }
}
