namespace MassTransit.PrometheusIntegration.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;


    [TestFixture]
    public class JobConsumer_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            IRequestClient<SubmitJob<TheJob>> requestClient = Bus.CreateRequestClient<SubmitJob<TheJob>>();

            var jobId = NewId.NextGuid();
            await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(30) }
            });

            await InactivityTask;

            await using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(text);

            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"AllocateJobSlot\"} 1"), "allocate");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSlotAllocated\"} 1"), "allocated");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSlotReleased\"} 1"), "released");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"StartJobAttempt\"} 1"), "startJobAttempt");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"StartJob\"} 1"), "startJob");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"JobSubmissionAccepted\"} 1"), "accepted");
            Assert.That(text.Contains("mt_publish_total{service_name=\"unit_test\",message_type=\"SubmitJob_TheJob\"} 1"), "submitTheJob");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"SubmitJob_TheJob\",consumer_type=\"SubmitJobConsumer_TheJob\"} 1"),
                "submit job");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"TheJob\",consumer_type=\"TestJobConsumer\"} 1"),
                "submit job");
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

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

        public JobConsumer_Specs()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(1);
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
