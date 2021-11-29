namespace MassTransit.KafkaIntegration.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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
                    });
                });
            });
            var provider = services.BuildServiceProvider();
            var healthCheckService = provider.GetRequiredService<HealthCheckService>();
            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>().ToArray();

            await healthCheckService.WaitForHealthStatus(HealthStatus.Unhealthy);

            try
            {
                await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

                await healthCheckService.WaitForHealthStatus(HealthStatus.Healthy);
            }
            finally
            {
                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(TestCancellationToken)));
                await provider.DisposeAsync();
            }
        }
    }
}
