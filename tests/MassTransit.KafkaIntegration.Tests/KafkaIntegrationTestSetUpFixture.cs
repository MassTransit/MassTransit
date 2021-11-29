using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(1)]


namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using RetryPolicies;


    [SetUpFixture]
    public class KafkaIntegrationTestSetUpFixture
    {
        [OneTimeSetUp]
        public async Task Before_any()
        {
            await CheckBrokerReady();
        }

        static Task CheckBrokerReady()
        {
            return Retry.Interval(10, 5000).Retry(async () =>
            {
                using var client = new CachedSchemaRegistryClient(new Dictionary<string, string>
                {
                    { "schema.registry.url", "localhost:8081" },
                });

                await client.GetAllSubjectsAsync();

                var clientConfig = new ClientConfig { BootstrapServers = "localhost:9092" };
                using var adminClient = new AdminClientBuilder(clientConfig).Build();

                adminClient.GetMetadata(TimeSpan.FromSeconds(60));
            });
        }
    }
}
