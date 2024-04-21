namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using NUnit.Framework;
    using TestFramework;


    public class ReceiveEndpointDependency_Specs
    {
        [Test]
        public async Task Should_not_receive_message_before_dependency_ready()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            var healthChecks = provider.GetService<HealthCheckService>();

            await harness.Start();

            await harness.Bus.Publish<DependentMessage>(new { });

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<DependentMessage>(cts.Token), Is.True);
                Assert.That(await harness.Consumed.Any<DependentMessage>(cts.Token), Is.False);
            });

            await healthChecks.WaitForHealthStatus(HealthStatus.Unhealthy);

            await harness.Bus.Publish<DependencyReadyMessage>(new { });

            await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Consumed.Any<DependencyReadyMessage>(cts.Token), Is.True);
                Assert.That(await harness.Consumed.Any<DependentMessage>(cts.Token), Is.True);
            });
        }

        static ServiceProvider SetupServiceCollection()
        {
            var services = new ServiceCollection()
                .AddSingleton<IReceiveEndpointDependency, TestDependency>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<DependencyConsumer>();
                    x.AddConsumer<DependentConsumer>(typeof(DependentConsumerDefinition));

                    x.AddTaskCompletionSource<bool>();
                });

            services.AddOptions<MassTransitHostOptions>().Configure(x => x.WaitUntilStarted = false);

            return services.BuildServiceProvider(true);
        }


        class DependentConsumerDefinition :
            ConsumerDefinition<DependentConsumer>
        {
            readonly IReceiveEndpointDependency _dependency;

            public DependentConsumerDefinition(IReceiveEndpointDependency dependency)
            {
                _dependency = dependency;
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<DependentConsumer> consumerConfigurator,
                IRegistrationContext context)
            {
                endpointConfigurator.AddDependency(_dependency);
            }
        }


        class TestDependency :
            IReceiveEndpointDependency
        {
            public TestDependency(TaskCompletionSource<bool> taskCompletionSource)
            {
                Ready = taskCompletionSource.Task;
            }

            public Task Ready { get; }
        }


        public interface DependencyReadyMessage
        {
        }


        public interface DependentMessage
        {
        }


        class DependencyConsumer :
            IConsumer<DependencyReadyMessage>
        {
            readonly TaskCompletionSource<bool> _taskCompletionSource;

            public DependencyConsumer(TaskCompletionSource<bool> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<DependencyReadyMessage> context)
            {
                _taskCompletionSource.TrySetResult(true);
                return Task.CompletedTask;
            }
        }


        class DependentConsumer :
            IConsumer<DependentMessage>
        {
            public Task Consume(ConsumeContext<DependentMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
