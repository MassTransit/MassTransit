namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;


    public abstract class Common_RequestClient_Outbox :
        InMemoryTestFixture
    {
        protected Common_RequestClient_Outbox()
        {
            SubsequentQueueName = "subsequent_queue";
            SubsequentQueueAddress = new Uri(BaseAddress, SubsequentQueueName);
        }

        protected Uri SubsequentQueueAddress { get; }
        string SubsequentQueueName { get; }

        protected abstract IRequestClient<InitialRequest> RequestClient { get; }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = RequestClient;

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new {Value = "World"});
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(SubsequentQueueName, cfg => cfg.ConfigureConsumer<SubsequentConsumer>(Registration));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumer<InitialConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddBus(context => BusControl);

            configurator.AddRequestClient<InitialRequest>(InputQueueAddress);
            configurator.AddRequestClient(typeof(SubsequentRequest), SubsequentQueueAddress);
        }


        protected class InitialConsumer :
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


        protected class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new {Value = $"Hello, {context.Message.Value}"});
            }
        }


        public interface InitialRequest
        {
            string Value { get; }
        }


        public interface InitialResponse
        {
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            string Value { get; }
        }
    }


    public abstract class Common_RequestClient_Outbox_Courier :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        protected Common_RequestClient_Outbox_Courier()
        {
            RequestQueueName = "activity-request";
            RequestQueueAddress = new Uri(BaseAddress, RequestQueueName);
        }

        protected Uri RequestQueueAddress { get; }
        string RequestQueueName { get; }

        protected abstract IBusRegistrationContext Registration { get; }

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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("send-request-activity-execute", endpointConfigurator =>
            {
                endpointConfigurator.UseInMemoryOutbox();

                endpointConfigurator.ConfigureExecuteActivity(Registration, typeof(SendRequestActivity));

                _executeAddress = endpointConfigurator.InputAddress;
            });

            configurator.ReceiveEndpoint(RequestQueueName, cfg =>
            {
                cfg.UseInMemoryOutbox();
                cfg.ConfigureConsumer<ActivityRequestConsumer>(Registration);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ActivityRequestConsumer>();
            configurator.AddExecuteActivity<SendRequestActivity, SendRequestArguments>();

            configurator.AddBus(context => BusControl);

            configurator.AddRequestClient<ActivityRequest>(RequestQueueAddress);
        }


        public class SendRequestActivity :
            IExecuteActivity<SendRequestArguments>
        {
            readonly IRequestClient<ActivityRequest> _client;

            public SendRequestActivity(IRequestClient<ActivityRequest> client)
            {
                _client = client;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<SendRequestArguments> context)
            {
                await _client.GetResponse<ActivityResponse>(new {Value = "Hello"});

                return context.Completed();
            }
        }


        public interface SendRequestArguments
        {
        }


        protected class ActivityRequestConsumer :
            IConsumer<ActivityRequest>
        {
            public Task Consume(ConsumeContext<ActivityRequest> context)
            {
                return context.RespondAsync<ActivityResponse>(new {Value = $"Hello, {context.Message.Value}"});
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
