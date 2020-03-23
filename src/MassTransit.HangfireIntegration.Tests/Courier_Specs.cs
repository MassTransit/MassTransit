namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework.Courier;
    using Testing;


    [TestFixture]
    public class When_using_retry_middleware_for_courier :
        HangfireInMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_faulted_routing_slip_event()
        {
            var testActivity = GetActivityContext<TestActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            Task<ConsumeContext<RoutingSlipFaulted>> handled = ConnectPublishHandler<RoutingSlipFaulted>();
            Task<ConsumeContext<RoutingSlipActivityCompensated>> compensated = ConnectPublishHandler<RoutingSlipActivityCompensated>(
                context => context.Message.ActivityName.Equals(testActivity.Name));

            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new {Value = "Hello"});
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new { });

            await Bus.Execute(builder.Build());

            await handled;

            await compensated;
        }

        [Test]
        public async Task Should_retry_and_eventually_succeed()
        {
            var testActivity = GetActivityContext<TestActivity>();
            var faultActivity = GetActivityContext<FirstFaultyActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed = ConnectPublishHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new {Value = "Hello"});
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new { });

            await Bus.Execute(builder.Build());

            await completed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseScheduledRedelivery(r => r.Intervals(200, 500));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
            AddActivityContext<FirstFaultyActivity, FaultyArguments, FaultyLog>(() => new FirstFaultyActivity());
        }
    }
}
