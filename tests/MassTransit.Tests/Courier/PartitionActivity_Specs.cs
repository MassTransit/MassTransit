namespace MassTransit.Tests.Courier
{
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_partitioning_by_activity_arguments :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_completed_event()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new { Value = "Hello" });

            await Bus.Execute(builder.Build());

            await completed;
        }

        IPartitioner _partitioner;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            testHarness.OnConfigureBus += ConfigureBus;

            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity(),
                h => h.UsePartitioner(_partitioner, args => args.Arguments.Value),
                h => h.UsePartitioner(_partitioner, args => args.Log.OriginalValue));
        }

        void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            _partitioner = configurator.CreatePartitioner(8);
        }
    }
}
