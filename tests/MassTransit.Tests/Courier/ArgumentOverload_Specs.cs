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
    public class Arguments_in_an_activity :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_override_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);

            Assert.AreEqual("Used", context.GetVariable<string>("Test"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
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
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<SetVariableActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Key = "Test",
                Value = "Used"
            });

            builder.AddVariable("Value", "Ignored");

            await Bus.Execute(builder.Build());

            await _completed;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<SetVariableActivity, SetVariableArguments>(() => new SetVariableActivity());
        }
    }


    [TestFixture]
    public class Arguments_missing_from_an_activity :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_use_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);

            Assert.AreEqual("Used", context.GetVariable<string>("Test"));
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
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<SetVariableActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Key = "Test" });

            builder.AddVariable("Value", "Used");

            await Bus.Execute(builder.Build());

            await _completed;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<SetVariableActivity, SetVariableArguments>(() => new SetVariableActivity());
        }
    }
}
