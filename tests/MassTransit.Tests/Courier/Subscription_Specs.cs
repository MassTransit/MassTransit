namespace MassTransit.Tests.Courier
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Serialization;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    public static class TestExtensionsForJson
    {
        public static string ToJsonString(this RoutingSlip routingSlip)
        {
            return JsonSerializer.Serialize(routingSlip);
        }

        public static RoutingSlip GetRoutingSlip(string json)
        {
            return JsonSerializer.Deserialize<RoutingSlip>(json, SystemTextJsonMessageSerializer.Options);
        }
    }


    [TestFixture]
    public class A_routing_slip_event_subscription
    {
        [Test]
        public void Should_serialize_properly()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity("a", new Uri("loopback://locahost/execute_a"));

            builder.AddSubscription(new Uri("loopback://localhost/events"), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            var routingSlip = builder.Build();

            var jsonString = routingSlip.ToJsonString();

            var loaded = TestExtensionsForJson.GetRoutingSlip(jsonString);

            Assert.AreEqual(RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted, loaded.Subscriptions[0].Events);
        }
    }


    [TestFixture]
    public class Subscription_without_variables :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_not_receive_the_routing_slip_activity_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.IsFalse(context.Message.Data.ContainsKey("OriginalValue"));
        }

        [Test]
        public async Task Should_not_receive_the_routing_slip_activity_variable()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.IsFalse(context.Message.Variables.ContainsKey("Variable"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public void Should_serialize_and_deserialize_properly()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity("a", new Uri("loopback://locahost/execute_a"));

            builder.AddSubscription(new Uri("loopback://localhost/events"), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            var routingSlip = builder.Build();

            var jsonString = routingSlip.ToJsonString();

            var loaded = TestExtensionsForJson.GetRoutingSlip(jsonString);

            Assert.AreEqual(RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted, loaded.Subscriptions[0].Events);
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;

        [OneTimeSetUp]
        public async Task Should_publish_the_completed_event()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.ActivityCompleted, RoutingSlipEventContents.None);

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });

            builder.AddVariable("Variable", "Knife");

            await Bus.Execute(builder.Build());
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
        }
    }
}
