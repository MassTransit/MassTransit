namespace MassTransit.Containers.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Flaky")]
    public class KillSwitch_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_be_degraded_after_too_many_exceptions()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory>(_ => LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
                {
                    x.AddConsumer<BadConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseKillSwitch(options => options
                            .SetActivationThreshold(5)
                            .SetTripThreshold(10)
                            .SetRestartTimeout(s: 5));

                        cfg.ConfigureEndpoints(context);
                    });
                });

            IServiceProvider provider = services.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();

            await healthChecks.WaitForHealthStatus(HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);

                using var scope = provider.CreateScope();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await Task.WhenAll(Enumerable.Range(0, 20).Select(x => publishEndpoint.Publish(new BadMessage())));

                await healthChecks.WaitForHealthStatus(HealthStatus.Degraded);

                await Task.WhenAll(Enumerable.Range(0, 20).Select(x => publishEndpoint.Publish(new GoodMessage())));

                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }


        class BadConsumer :
            IConsumer<BadMessage>,
            IConsumer<GoodMessage>
        {
            public Task Consume(ConsumeContext<BadMessage> context)
            {
                throw new IntentionalTestException("Trying to trigger the kill switch");
            }

            public Task Consume(ConsumeContext<GoodMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        class GoodMessage
        {
        }


        class BadMessage
        {
        }


        public KillSwitch_Specs()
            : base(new InMemoryTestHarness())
        {
            TestTimeout = TimeSpan.FromMinutes(1);
        }
    }
}
