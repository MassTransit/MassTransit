namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public record DemoArguments(Guid Id);


    public record DemoEvent(Guid Id);


    public class DemoActivityTests :
        InMemoryActivityTestFixture
    {
        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<DemoActivity, DemoArguments>(() => new DemoActivity());
        }

        [Test]
        public async Task Demo_should_not_fail_to_serialize()
        {
            var activity = GetActivityContext<DemoActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed = InMemoryTestHarness.SubscribeHandler<RoutingSlipCompleted>();
            Task<ConsumeContext<RoutingSlipActivityCompleted>> activityCompleted = InMemoryTestHarness.SubscribeHandler<RoutingSlipActivityCompleted>();

            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(InMemoryTestHarness.BusAddress, RoutingSlipEvents.All);
            builder.AddActivity(activity.Name, activity.ExecuteUri, new DemoArguments(Guid.NewGuid()));

            await InMemoryTestHarness.Bus.Execute(builder.Build());

            await completed;

            ConsumeContext<RoutingSlipActivityCompleted> context = await activityCompleted!;

            Assert.True(await InMemoryTestHarness.Published.Any<DemoEvent>());
            Assert.AreEqual(trackingNumber, context.Message.TrackingNumber);
        }
    }


    public class DemoActivity : IExecuteActivity<DemoArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<DemoArguments> context)
        {
            await Task.Delay(500);

            await context.Publish(new DemoEvent(context.Arguments.Id)).ConfigureAwait(false);
            return context.Completed();
        }
    }
}
