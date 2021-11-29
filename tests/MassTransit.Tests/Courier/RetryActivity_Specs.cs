namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_using_retry_middleware_for_courier :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_handle_the_failed_to_compensate_event()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipActivityCompensationFailed>> handledCompensationFailure =
                await ConnectPublishHandler<RoutingSlipActivityCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipCompensationFailed>> handledRoutingSlipFailure =
                await ConnectPublishHandler<RoutingSlipCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            var testActivity = GetActivityContext<TestActivity>();
            var faultyCompensateActivity = GetActivityContext<FaultyCompensateActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            builder.AddVariable("Value", "Hello");
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handledRoutingSlipFailure;

            await handledCompensationFailure;
        }

        [Test]
        public async Task Should_publish_the_faulted_routing_slip_event()
        {
            var testActivity = GetActivityContext<TestActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            Task<ConsumeContext<RoutingSlipFaulted>> handled = await ConnectPublishHandler<RoutingSlipFaulted>();
            Task<ConsumeContext<RoutingSlipActivityCompensated>> compensated = await ConnectPublishHandler<RoutingSlipActivityCompensated>(
                context => context.Message.ActivityName.Equals(testActivity.Name));

            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new { });

            await Bus.Execute(builder.Build());

            await handled;

            await compensated;
        }

        [Test]
        public async Task Should_retry_and_eventually_compensate()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var testActivity = GetActivityContext<TestActivity>();
            var faultyCompensateActivity = GetActivityContext<FirstFaultyCompensateActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            Task<ConsumeContext<RoutingSlipActivityCompensated>> compensated = await ConnectPublishHandler<RoutingSlipActivityCompensated>(
                context => context.Message.ActivityName.Equals(faultyCompensateActivity.Name));

            Task<ConsumeContext<RoutingSlipFaulted>> routingSlipFailure =
                await ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            builder.AddVariable("Value", "Hello");
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await routingSlipFailure;

            await compensated;
        }

        [Test]
        public async Task Should_retry_and_eventually_succeed()
        {
            var testActivity = GetActivityContext<TestActivity>();
            var faultActivity = GetActivityContext<FirstFaultyActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed = await ConnectPublishHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new { });

            await Bus.Execute(builder.Build());

            await completed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageRetry(r => r.Immediate(2));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
            AddActivityContext<FirstFaultyActivity, FaultyArguments, FaultyLog>(() => new FirstFaultyActivity());
            AddActivityContext<FaultyCompensateActivity, TestArguments, TestLog>(() => new FaultyCompensateActivity());
            AddActivityContext<FirstFaultyCompensateActivity, TestArguments, TestLog>(() => new FirstFaultyCompensateActivity());
        }
    }
}
