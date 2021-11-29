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
    public class When_an_itinerary_is_revised :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_complete_the_additional_item()
        {
            var trackingNumber = Guid.NewGuid();

            var testActivity = GetActivityContext<TestActivity>();
            var reviseActivity = GetActivityContext<ReviseItineraryActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed =
                await ConnectPublishHandler<RoutingSlipCompleted>(context => context.Message.TrackingNumber == trackingNumber);

            Task<ConsumeContext<RoutingSlipActivityCompleted>> testActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(testActivity.Name));

            Task<ConsumeContext<RoutingSlipActivityCompleted>> reviseActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(reviseActivity.Name));

            Task<ConsumeContext<RoutingSlipRevised>> revised = await ConnectPublishHandler<RoutingSlipRevised>(
                context => context.Message.TrackingNumber == trackingNumber);

            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddActivity(reviseActivity.Name, reviseActivity.ExecuteUri, new { Value = "Time to add a new item!" });

            await Bus.Execute(builder.Build());

            await completed;
            await testActivityCompleted;
            await reviseActivityCompleted;

            ConsumeContext<RoutingSlipRevised> revisions = await revised;
            Assert.AreEqual(0, revisions.Message.DiscardedItinerary.Length);
        }

        [Test]
        public async Task Should_continue_with_the_source_itinerary()
        {
            var trackingNumber = Guid.NewGuid();

            var testActivity = GetActivityContext<TestActivity>();
            var reviseActivity = GetActivityContext<ReviseItineraryActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed =
                await ConnectPublishHandler<RoutingSlipCompleted>(context => context.Message.TrackingNumber == trackingNumber);

            Task<ConsumeContext<RoutingSlipActivityCompleted>> testActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(testActivity.Name));

            Task<ConsumeContext<RoutingSlipActivityCompleted>> reviseActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(reviseActivity.Name));

            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddActivity(reviseActivity.Name, reviseActivity.ExecuteUri, new { Value = "Time to add a new item!" });

            await Bus.Execute(builder.Build());

            await completed;
            await reviseActivityCompleted;
            ConsumeContext<RoutingSlipActivityCompleted> testActivityResult = await testActivityCompleted;

            testActivityResult.GetArgument<string>("Value").ShouldBe("Added");
        }

        [Test]
        public async Task Should_immediately_complete_an_empty_list()
        {
            var trackingNumber = Guid.NewGuid();

            var testActivity = GetActivityContext<TestActivity>();
            var reviseActivity = GetActivityContext<ReviseToEmptyItineraryActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed =
                await ConnectPublishHandler<RoutingSlipCompleted>(context => context.Message.TrackingNumber == trackingNumber);

            Task<ConsumeContext<RoutingSlipActivityCompleted>> testActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(testActivity.Name));

            Task<ConsumeContext<RoutingSlipActivityCompleted>> reviseActivityCompleted = await ConnectPublishHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(reviseActivity.Name));

            Task<ConsumeContext<RoutingSlipRevised>> revised = await ConnectPublishHandler<RoutingSlipRevised>(
                context => context.Message.TrackingNumber == trackingNumber);

            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddActivity(reviseActivity.Name, reviseActivity.ExecuteUri, new { Value = "Time to remove any remaining items!" });
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });

            await Bus.Execute(builder.Build());

            await completed;
            await reviseActivityCompleted;

            ConsumeContext<RoutingSlipRevised> revisions = await revised;
            Assert.AreEqual(1, revisions.Message.DiscardedItinerary.Length);
            Assert.AreEqual(0, revisions.Message.Itinerary.Length);

            testActivityCompleted.Wait(TimeSpan.FromSeconds(3)).ShouldBe(false);
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<ReviseToEmptyItineraryActivity, TestArguments, TestLog>(
                () => new ReviseToEmptyItineraryActivity());
            AddActivityContext<ReviseWithNoChangeItineraryActivity, TestArguments, TestLog>(
                () => new ReviseWithNoChangeItineraryActivity());

            var testActivity = GetActivityContext<TestActivity>();
            AddActivityContext<ReviseItineraryActivity, TestArguments, TestLog>(
                () =>
                    new ReviseItineraryActivity(
                        x => x.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Added" })));
        }
    }
}
