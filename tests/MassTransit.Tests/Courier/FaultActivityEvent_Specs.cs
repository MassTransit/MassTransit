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
    public class Executing_a_faulting_routing_slip_with_compensating_activities :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_compensate_completed_activity()
        {
            ConsumeContext<RoutingSlipActivityCompensated> compensated = await _activityCompensated;
            ConsumeContext<RoutingSlipActivityCompleted> completed = await _activityCompleted;

            Assert.AreEqual(completed.Message.TrackingNumber, compensated.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_compensate_first_activity()
        {
            ConsumeContext<RoutingSlipActivityCompensated> context = await _activityCompensated;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_complete_activity_with_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Hello", context.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async Task Should_complete_first_activity()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_complete_second_activity()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _secondActivityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_complete_with_variable()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Knife", context.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_fault_activity_with_variable()
        {
            ConsumeContext<RoutingSlipActivityFaulted> context = await _activityFaulted;

            Assert.AreEqual("Knife", context.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_fault_third_activity()
        {
            ConsumeContext<RoutingSlipActivityFaulted> context = await _activityFaulted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_fault_with_variable()
        {
            ConsumeContext<RoutingSlipFaulted> context = await _faulted;

            Assert.AreEqual("Knife", context.GetVariable<string>("Variable"));
        }

        Task<ConsumeContext<RoutingSlipFaulted>> _faulted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipActivityFaulted>> _activityFaulted;
        Guid _trackingNumber;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _secondActivityCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompensated>> _activityCompensated;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _faulted = SubscribeHandler<RoutingSlipFaulted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>(x => x.Message.ActivityName.Equals("Test"));
            _activityCompensated = SubscribeHandler<RoutingSlipActivityCompensated>(x => x.Message.ActivityName.Equals("Test"));
            _secondActivityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>(x => x.Message.ActivityName.Equals("SecondTest"));
            _activityFaulted = SubscribeHandler<RoutingSlipActivityFaulted>(x => x.Message.ActivityName.Equals("Faulty"));

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });

            testActivity = GetActivityContext<SecondTestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            testActivity = GetActivityContext<FaultyActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");

            await Bus.Execute(builder.Build());
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
        }
    }
}
