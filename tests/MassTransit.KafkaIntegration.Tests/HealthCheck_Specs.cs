namespace MassTransit.KafkaIntegration.Tests
{
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class HealthCheck_Specs :
        InMemoryTestFixture
    {
        const string Topic = "health-check";

        [Test]
        public async Task Should_be_healthy()
        {
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddRider(rider =>
                    {
                        rider.UsingKafka((_, k) =>
                        {
                            k.TopicEndpoint<Null, Ignore>(Topic, nameof(HealthCheck_Specs), _ =>
                            {
                            });
                        });
                    });
                }).BuildServiceProvider();
            var harness = provider.GetTestHarness();

            var healthCheckService = provider.GetRequiredService<HealthCheckService>();

            await healthCheckService.WaitForHealthStatus(HealthStatus.Unhealthy);

            await harness.Start();

            await healthCheckService.WaitForHealthStatus(HealthStatus.Healthy);
        }
    }
}
