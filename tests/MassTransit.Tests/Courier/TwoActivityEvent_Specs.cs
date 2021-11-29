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

            Assert.AreEqual("Hello", activityCompleted.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async Task Should_include_the_variable_set_by_the_activity()
        {
            ConsumeContext<RoutingSlipCompleted> completed = await _completed;

            Assert.AreEqual("Hello, World!", completed.GetVariable<string>("Value"));
        }

        [Test]
        public async Task Should_include_the_variables_of_the_completed_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> completed = await _completed;

            Assert.AreEqual("Knife", completed.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_include_the_variables_with_the_activity_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> activityCompleted = await _firstActivityCompleted;

            Assert.AreEqual("Knife", activityCompleted.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_receive_the_first_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            var completed = (await _completed).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);

            Console.WriteLine("Duration: {0}", completed.Duration);

            Assert.IsFalse(completed.Variables.ContainsKey("ToBeRemoved"));
        }

        [Test]
        public async Task Should_receive_the_second_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _secondActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
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
