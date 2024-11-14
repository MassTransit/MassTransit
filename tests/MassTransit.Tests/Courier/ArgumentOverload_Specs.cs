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

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.GetVariable<string>("Test"), Is.EqualTo("Used"));
            });
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        #pragma warning restore NUnit1032
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

            Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));
        }

        [Test]
        public async Task Should_use_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.GetVariable<string>("Test"), Is.EqualTo("Used"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        #pragma warning restore NUnit1032
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


    [TestFixture]
    public class Arguments_with_null_values_from_an_activity :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));
        }

        [Test]
        public async Task Should_use_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.GetVariable<string>("Test"), Is.EqualTo("Used"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        #pragma warning restore NUnit1032
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
                Value = (string)null
            });

            builder.AddVariable("Value", "Used");

            await Bus.Execute(builder.Build());

            await _completed;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<SetVariableActivity, SetVariableArguments>(() => new SetVariableActivity());
        }
    }


    [TestFixture]
    public class Arguments_with_default_values_from_an_activity :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.GetArgument<Guid>("GuidValue"), Is.EqualTo(_trackingNumber));
            });
        }

        [Test]
        public async Task Should_use_variables_in_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.GetVariable<string>("Test"), Is.EqualTo("Used"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        #pragma warning restore NUnit1032
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
                Value = (string)null,
                GuidValue = Guid.Empty,
            });

            builder.AddVariable("Value", "Used");
            builder.AddVariable("GuidValue", _trackingNumber);

            await Bus.Execute(builder.Build());

            await _completed;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<SetVariableActivity, SetVariableArguments>(() => new SetVariableActivity());
        }
    }
}
