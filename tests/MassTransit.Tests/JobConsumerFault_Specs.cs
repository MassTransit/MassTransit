namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using JobConsumerTests;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace JobConsumerTests
    {
        using System.Threading.Tasks;


        public interface ICustomDependency
        {
        }


        public class OddJobFaultConsumer :
            IJobConsumer<OddJob>
        {
            public OddJobFaultConsumer(ICustomDependency dependency)
            {
            }

            public async Task Run(JobContext<OddJob> context)
            {
                await Task.Delay(context.Job.Duration, context.CancellationToken);
            }
        }
    }


    [TestFixture]
    public class JobConsumerFault_Specs
    {
        [Test]
        public async Task Should_detect_the_faulted_job()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            Assert.That(response.Message.JobId, Is.EqualTo(jobId));

            Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

            Assert.That(await harness.Published.Any<JobFaulted>(), Is.True);
            Assert.That(await harness.Published.Any<Fault<OddJob>>(), Is.True);
        }

        static ServiceProvider SetupServiceCollection()
        {
            var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobFaultConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();

                        var options = new ServiceInstanceOptions()
                            .SetEndpointNameFormatter(context.GetService<IEndpointNameFormatter>() ??
                                DefaultEndpointNameFormatter.Instance);

                        cfg.ServiceInstance(options, instance =>
                        {
                            instance.ConfigureJobServiceEndpoints();

                            instance.ConfigureEndpoints(context, f => f.Include<OddJobFaultConsumer>());
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            return provider;
        }
    }
}
