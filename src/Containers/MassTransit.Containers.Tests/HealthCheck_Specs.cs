namespace MassTransit.Containers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class HealthCheck_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_healthy_with_configured_receive_endpoints()
        {
            var collection = new ServiceCollection();
            collection.AddLogging();
            collection.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            collection.AddMassTransit(configurator =>
                {
                    configurator.AddBus(p => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
                    {
                        cfg.UseHealthCheck(p);
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.UseMessageRetry(r => r.Immediate(5));
                        });
                    }));
                })
                .AddMassTransitHostedService();

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            var hostedServices = provider.GetRequiredService<IEnumerable<IHostedService>>();

            var result = await healthChecks.CheckHealthAsync(TestCancellationToken);
            Assert.That(result.Status == HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                do
                {
                    result = await healthChecks.CheckHealthAsync(TestCancellationToken);

                    Console.WriteLine("Health Check: {0}",
                        string.Join(", ", result.Entries.Select(x => string.Join("=", x.Key, x.Value.Status))));

                    await Task.Delay(100, TestCancellationToken);
                }
                while (result.Status == HealthStatus.Unhealthy);

                Assert.That(result.Status == HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }
    }
}
