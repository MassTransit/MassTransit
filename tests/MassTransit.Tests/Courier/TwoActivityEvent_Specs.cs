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
    public class Executing_a_routing_slip_with_two_activities :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_include_the_activity_log_data()
        {
            ConsumeContext<RoutingSlipActivityCompleted> activityCompleted = await _firstActivityCompleted;

            Assert.That(activityCompleted.GetResult<string>("OriginalValue"), Is.EqualTo("Hello"));
        }

        [Test]
        public async Task Should_include_the_variable_set_by_the_activity()
        {
            ConsumeContext<RoutingSlipCompleted> completed = await _completed;

            Assert.That(completed.GetVariable<string>("Value"), Is.EqualTo("Hello, World!"));
        }

        [Test]
        public async Task Should_include_the_variables_of_the_completed_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> completed = await _completed;

            Assert.That(completed.GetVariable<string>("Variable"), Is.EqualTo("Knife"));
        }

        [Test]
        public async Task Should_include_the_variables_with_the_activity_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> activityCompleted = await _firstActivityCompleted;

            Assert.That(activityCompleted.GetVariable<string>("Variable"), Is.EqualTo("Knife"));
        }

        [Test]
        public async Task Should_receive_the_first_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _firstActivityCompleted).Message;

            Assert.That(activityCompleted.TrackingNumber, Is.EqualTo(_routingSlip.TrackingNumber));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            var completed = (await _completed).Message;

            Assert.That(completed.TrackingNumber, Is.EqualTo(_routingSlip.TrackingNumber));

            Console.WriteLine("Duration: {0}", completed.Duration);

            Assert.That(completed.Variables.ContainsKey("ToBeRemoved"), Is.False);
        }

        [Test]
        public async Task Should_receive_the_second_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _secondActivityCompleted).Message;

            Assert.That(activityCompleted.TrackingNumber, Is.EqualTo(_routingSlip.TrackingNumber));
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _firstActivityCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _secondActivityCompleted;
        RoutingSlip _routingSlip;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint("events", x =>
            {
                _completed = Handled<RoutingSlipCompleted>(x);

                var testActivity = GetActivityContext<TestActivity>();
                var secondActivity = GetActivityContext<SecondTestActivity>();

                _firstActivityCompleted =
                    Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(testActivity.Name));
                _secondActivityCompleted =
                    Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(secondActivity.Name));
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);
            builder.AddVariable("ToBeRemoved", "Existing");

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);
        }
    }
}
