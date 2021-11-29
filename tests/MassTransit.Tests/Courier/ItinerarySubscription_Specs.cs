namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_an_activity_adds_a_subscription :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_continue_with_the_source_itinerary()
        {
            _trackingNumber = Guid.NewGuid();

            var reviseActivity = GetActivityContext<ReviseItineraryActivity>();

            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddActivity(reviseActivity.Name, reviseActivity.ExecuteUri, new { Value = "Time to add a new item!" });

            await Bus.Execute(builder.Build());

            await _completed;
            await _reviseActivityCompleted;
            ConsumeContext<RoutingSlipActivityCompleted> testActivityResult = await _testActivityCompleted;

            testActivityResult.GetArgument<string>("Value").ShouldBe("Added");

            ConsumeContext<RoutingSlipActivityCompleted> consumeContext = await _handled;

            Assert.That(consumeContext.GetArgument<string>("Value"), Is.EqualTo("Added"));
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _handled;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _testActivityCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _reviseActivityCompleted;
        Guid _trackingNumber;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint("events", x =>
            {
                var testActivity = GetActivityContext<TestActivity>();
                var reviseActivity = GetActivityContext<ReviseItineraryActivity>();

                _completed = Handled<RoutingSlipCompleted>(x, context => context.Message.TrackingNumber == _trackingNumber);

                _testActivityCompleted = Handled<RoutingSlipActivityCompleted>(x,
                    context => context.Message.TrackingNumber == _trackingNumber && context.Message.ActivityName.Equals(testActivity.Name));

                _reviseActivityCompleted = Handled<RoutingSlipActivityCompleted>(x,
                    context => context.Message.TrackingNumber == _trackingNumber && context.Message.ActivityName.Equals(reviseActivity.Name));
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            var testActivity = GetActivityContext<TestActivity>();

            _handled = Handled<RoutingSlipActivityCompleted>(configurator, x => x.Message.ActivityName == testActivity.Name);
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());

            var testActivity = GetActivityContext<TestActivity>();

            AddActivityContext<ReviseItineraryActivity, TestArguments, TestLog>(() => new ReviseItineraryActivity(x =>
            {
                x.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Added" });
                x.AddSubscription(InputQueueAddress, RoutingSlipEvents.ActivityCompleted | RoutingSlipEvents.Supplemental, RoutingSlipEventContents.All,
                    testActivity.Name);
            }));
        }
    }
}
