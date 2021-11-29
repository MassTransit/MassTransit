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
    public class When_an_activity_uses_an_address :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_compensate_with_the_log()
        {
            Task<ConsumeContext<RoutingSlipFaulted>> faulted = SubscribeHandler<RoutingSlipFaulted>();
            Task<ConsumeContext<RoutingSlipActivityCompleted>> activity = SubscribeHandler<RoutingSlipActivityCompleted>();
            Task<ConsumeContext<RoutingSlipActivityCompensated>> activityCompensated = SubscribeHandler<RoutingSlipActivityCompensated>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<AddressActivity>();
            var faultyActivity = GetActivityContext<FaultyActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Address = new Uri("http://google.com/") });
            builder.AddActivity(faultyActivity.Name, faultyActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await faulted;

            ConsumeContext<RoutingSlipActivityCompleted> consumeContext = await activity;

            Assert.AreEqual(new Uri("http://google.com/"), consumeContext.GetResult<string>("UsedAddress"));

            ConsumeContext<RoutingSlipActivityCompensated> context = await activityCompensated;

            Assert.AreEqual(new Uri("http://google.com/"), context.GetResult<string>("UsedAddress"));
        }

        [Test]
        public async Task Should_publish_the_completed_event()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();
            Task<ConsumeContext<RoutingSlipActivityCompleted>> activity = SubscribeHandler<RoutingSlipActivityCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<AddressActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);

            builder.SetVariables(new { Address = new Uri("http://google.com/") });

            await Bus.Execute(builder.Build());

            await completed;

            ConsumeContext<RoutingSlipActivityCompleted> consumeContext = await activity;

            Assert.AreEqual(new Uri("http://google.com/"), consumeContext.GetResult<string>("UsedAddress"));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<AddressActivity, AddressArguments, AddressLog>(() => new AddressActivity());
            AddActivityContext<FaultyActivity, FaultyArguments>(() => new FaultyActivity());
        }
    }
}
