namespace MassTransit.PrometheusIntegration.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Courier.Contracts;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;
    using TestFramework.Courier;
    using Testing;
    using Testing.Implementations;


    [TestFixture]
    public class ActivityMetric_Specs :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_complete_with_metrics_available()
        {
            await _completed;

            await _activityMonitor.AwaitBusInactivity(TestCancellationToken);

            await using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(text);

            Assert.That(text.Contains("mt_activity_execute_total{service_name=\"unit_test\",activity_name=\"SecondTest\",argument_type=\"Test\"} 1"));
            Assert.That(text.Contains("mt_activity_execute_total{service_name=\"unit_test\",activity_name=\"Test\",argument_type=\"Test\"} 1"));
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        RoutingSlip _routingSlip;
        IBusActivityMonitor _activityMonitor;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UsePrometheusMetrics(serviceName: "unit_test");

            configurator.ReceiveEndpoint("events", x =>
            {
                _completed = Handled<RoutingSlipCompleted>(x);

                var testActivity = GetActivityContext<TestActivity>();
                var secondActivity = GetActivityContext<SecondTestActivity>();

                Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(testActivity.Name));
                Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(secondActivity.Name));
            });
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
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
