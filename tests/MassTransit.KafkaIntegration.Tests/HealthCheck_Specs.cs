namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class HealthCheck_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_healthy()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");
                        k.TopicEndpoint<Null, Ignore>("test", nameof(HealthCheck_Specs), c =>
                        {
                        });

                        k.UseHealthCheck(context);
                    });
                });
            });
            services.AddMassTransitHostedService();
            var provider = services.BuildServiceProvider();
            var healthCheckService = provider.GetRequiredService<HealthCheckService>();
            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>();

            var result = await healthCheckService.CheckHealthAsync(TestCancellationToken);
            Assert.That(result.Status == HealthStatus.Unhealthy);

            try
            {
                await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                do
                {
                    result = await healthCheckService.CheckHealthAsync(TestCancellationToken);

                    Console.WriteLine("Health Check: {0}",
                        string.Join(", ", result.Entries.Select(x => string.Join("=", x.Key, x.Value.Status))));

                    await Task.Delay(100, TestCancellationToken);
                }
                while (result.Status == HealthStatus.Unhealthy);

                Assert.That(result.Status == HealthStatus.Healthy);
            }
            finally
            {
                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(TestCancellationToken)));
                await provider.DisposeAsync();
            }
        }
    }
}
