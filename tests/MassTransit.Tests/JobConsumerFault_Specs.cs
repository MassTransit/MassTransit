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


        public class CustomDependency :
            ICustomDependency
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
                if (context.RetryAttempt > 0)
                    await Task.Delay(context.Job.Duration, context.CancellationToken);
                else
                    throw new InvalidOperationException("Failing the first time, for fun");
            }
        }
    }


    [TestFixture]
    public class JobConsumerFault_Specs
    {
        [Test]
        public async Task Should_detect_the_faulted_job()
        {
            await using var provider = SetupServiceCollection(x => x.AddConsumer<OddJobFaultConsumer>());

            var harness = provider.GetTestHarness();

            await harness.Start();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(response.Message.JobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobFaulted>(), Is.True);
                Assert.That(await harness.Published.Any<Fault<OddJob>>(), Is.True);
            });
        }

        [Test]
        public async Task Should_retry_the_faulted_job_and_pass_the_second_time()
        {
            await using var provider = SetupServiceCollection(x =>
            {
                x.AddConsumer<OddJobFaultConsumer>(c => c.Options<JobOptions<OddJob>>(options => options.SetRetry(r => r.Immediate(2))));
                x.AddScoped<ICustomDependency, CustomDependency>();
            });

            var harness = provider.GetTestHarness();

            await harness.Start();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            var duration = TimeSpan.FromSeconds(2);

            var submittedJobId = await client.SubmitJob(jobId, new { Duration = duration }, properties => properties.Set("Variable", "Knife"));

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(submittedJobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);

                IPublishedMessage<JobCompleted<OddJob>> completed = await harness.Published.SelectAsync<JobCompleted<OddJob>>().FirstOrDefault();

                Assert.That(completed.Context.Message.Job.Duration, Is.EqualTo(duration));
            });
        }

        static ServiceProvider SetupServiceCollection(Action<IBusRegistrationConfigurator> configure = null)
        {
            var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    configure?.Invoke(x);

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.SetKebabCaseEndpointNameFormatter();

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

            return provider;
        }
    }
}
