namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EventStore.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TestFramework;


    public class HealthCheck_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_healthcheck_specs_test";

        [Test]
        public async Task Should_be_healthy()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            _ = services.AddSingleton<EventStoreClient>((provider) =>
            {
                var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
                settings.ConnectionName = "MassTransit Test Connection";

                return new EventStoreClient(settings);
            });

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.UsingEventStoreDb((context, esdb) =>
                    {
                        esdb.CatchupSubscription(StreamName.AllStream, SubscriptionName, c =>
                        {
                            c.UseEventStoreDbCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));
                        });
                    });
                });
            });
            services.AddMassTransitHostedService();
            var provider = services.BuildServiceProvider();
            var healthCheckService = provider.GetRequiredService<HealthCheckService>();
            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>();

            await WaitForHealthStatus(healthCheckService, HealthStatus.Unhealthy);

            try
            {
                await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

                await WaitForHealthStatus(healthCheckService, HealthStatus.Healthy);
            }
            finally
            {
                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(TestCancellationToken)));
                await provider.DisposeAsync();
            }
        }

        async Task WaitForHealthStatus(HealthCheckService healthChecks, HealthStatus expectedStatus)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            HealthReport result;
            do
            {
                result = await healthChecks.CheckHealthAsync(TestCancellationToken);

                await Task.Delay(100, TestCancellationToken);
            }
            while (result.Status != expectedStatus);

            if (result.Status != expectedStatus)
                await TestContext.Out.WriteLineAsync(FormatHealthCheck(result));

            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }

        static string FormatHealthCheck(HealthReport result)
        {
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(entry => new JProperty(entry.Key, new JObject(
                    new JProperty("status", entry.Value.Status.ToString()),
                    new JProperty("description", entry.Value.Description),
                    new JProperty("data", JObject.FromObject(entry.Value.Data))))))));

            return json.ToString(Formatting.Indented);
        }
    }
}
