namespace MassTransit.PrometheusIntegration.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Definition;
    using JobService;
    using JobService.Configuration;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;
    using Testing;
    using Testing.Indicators;


    [TestFixture]
    public class JobConsumer_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            IRequestClient<SubmitJob<TheJob>> requestClient = Bus.CreateRequestClient<SubmitJob<TheJob>>();

            var jobId = NewId.NextGuid();
            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new {Duration = TimeSpan.FromSeconds(30)}
            });

            await _activityMonitor.AwaitBusInactivity(TestCancellationToken);

            using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(text);

            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"AllocateJobSlot\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSlotAllocated\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSlotReleased\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobAttemptCreated\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"StartJobAttempt\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"StartJob\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSubmissionAccepted\"} 1"), "send");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"SubmitJob_TheJob\"} 1"), "send");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"SubmitJob_TheJob\",consumer_type=\"SubmitJobConsumer_TheJob\"} 1"),
                "submit job");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"TheJob\",consumer_type=\"TestJobConsumer\"} 1"),
                "submit job");
        }

        IBusActivityMonitor _activityMonitor;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UsePrometheusMetrics(serviceName: "unit_test");

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints();

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<TheJob>(), e =>
                {
                    e.Consumer(() => new TestJobConsumer(), cfg =>
                    {
                        cfg.Options<JobOptions<TheJob>>(jobOptions => jobOptions.SetJobTimeout(TimeSpan.FromSeconds(90)));
                    });
                });
            });
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new TestJobConsumer());
        }


        public class TestJobConsumer :
            IJobConsumer<TheJob>
        {
            public Task Run(JobContext<TheJob> context)
            {
                return Task.CompletedTask;
            }
        }


        public interface TheJob
        {
            TimeSpan Duration { get; }
        }
    }
}
