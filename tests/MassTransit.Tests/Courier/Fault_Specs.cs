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
    public class When_an_activity_faults :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_capture_a_thrown_exception()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipFaulted>> handled =
                await ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            var faultActivity = GetActivityContext<NastyFaultyActivity>();

            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handled;
        }

        [Test]
        public async Task Should_compensate_both_activities()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipFaulted>> handled =
                await ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            var testActivity = GetActivityContext<TestActivity>();
            var faultActivity = GetActivityContext<NastyFaultyActivity>();

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello Again!" });
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello Again!" });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handled;
        }

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
        public async Task Should_handle_the_failed_to_compensate_event_via_subscription()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipActivityCompensationFailed>> handledCompensationFailure =
                SubscribeHandler<RoutingSlipActivityCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipCompensationFailed>> handledRoutingSlipFailure =
                SubscribeHandler<RoutingSlipCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            var testActivity = GetActivityContext<TestActivity>();
            var faultyCompensateActivity = GetActivityContext<FaultyCompensateActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            builder.AddVariable("Value", "Hello");
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            builder.AddSubscription(
                Bus.Address,
                RoutingSlipEvents.CompensationFailed,
                RoutingSlipEventContents.All);

            builder.AddSubscription(
                Bus.Address,
                RoutingSlipEvents.ActivityCompensationFailed,
                RoutingSlipEventContents.All);

            await Bus.Execute(builder.Build());

            await handledRoutingSlipFailure;

            await handledCompensationFailure;
        }

        [Test]
        public async Task Should_publish_the_faulted_routing_slip_event()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var testActivity = GetActivityContext<TestActivity>();
            var secondTestActivity = GetActivityContext<SecondTestActivity>();
            var faultActivity = GetActivityContext<FaultyActivity>();

            Task<ConsumeContext<RoutingSlipFaulted>> handled =
                await ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipActivityCompensated>> compensated = await ConnectPublishHandler<RoutingSlipActivityCompensated>(
                context => context.Message.ActivityName.Equals(testActivity.Name));

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });
            builder.AddActivity(secondTestActivity.Name, secondTestActivity.ExecuteUri, new { Value = "Hello Again!" });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new { });

            await Bus.Execute(builder.Build());

            await handled;

            await compensated;
        }

        [Test]
        public async Task Should_publish_the_faulted_routing_slip_event_and_set_variables()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var testActivity = GetActivityContext<SetVariablesFaultyActivity>();

            Task<ConsumeContext<RoutingSlipFaulted>> handled =
                await ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            ConsumeContext<RoutingSlipFaulted> context = await handled;

            Assert.AreEqual("Data", context.GetVariable<string>("Test"));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
            AddActivityContext<FaultyCompensateActivity, TestArguments, TestLog>(() => new FaultyCompensateActivity());
            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
            AddActivityContext<NastyFaultyActivity, FaultyArguments, FaultyLog>(() => new NastyFaultyActivity());
            AddActivityContext<SetVariablesFaultyActivity, SetVariablesFaultyArguments>(() => new SetVariablesFaultyActivity());
        }
    }
}
