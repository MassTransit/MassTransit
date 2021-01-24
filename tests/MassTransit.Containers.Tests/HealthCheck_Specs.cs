namespace MassTransit.Containers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class HealthCheck_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_degraded_after_stopping_a_connected_endpoint()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILoggerFactory>(_ => LoggerFactory);
            collection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            collection.AddMassTransit(configurator =>
                {
                    configurator.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.UseMessageRetry(r => r.Immediate(5));
                        });
                    });
                })
                .AddMassTransitHostedService();

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            var hostedServices = provider.GetRequiredService<IEnumerable<IHostedService>>();

            await WaitForHealthStatus(healthChecks, HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await WaitForHealthStatus(healthChecks, HealthStatus.Healthy);

                var busControl = provider.GetRequiredService<IBusControl>();

                var endpointHandle = busControl.ConnectReceiveEndpoint("another-queue", x =>
                {
                });

                await endpointHandle.Ready;

                await WaitForHealthStatus(healthChecks, HealthStatus.Healthy);

                using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await endpointHandle.ReceiveEndpoint.Stop(stop.Token);

                await WaitForHealthStatus(healthChecks, HealthStatus.Degraded);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }

        [Test]
        public async Task Should_be_healthy_after_restarting()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILoggerFactory>(_ => LoggerFactory);
            collection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            collection.AddMassTransit(configurator =>
                {
                    configurator.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.UseMessageRetry(r => r.Immediate(5));
                        });
                    });
                })
                .AddMassTransitHostedService();

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            var hostedServices = provider.GetRequiredService<IEnumerable<IHostedService>>();

            await WaitForHealthStatus(healthChecks, HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await WaitForHealthStatus(healthChecks, HealthStatus.Healthy);

                var busControl = provider.GetRequiredService<IBusControl>();

                using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await busControl.StopAsync(stop.Token);

                await WaitForHealthStatus(healthChecks, HealthStatus.Unhealthy);

                using var start = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await busControl.StartAsync(start.Token);

                await WaitForHealthStatus(healthChecks, HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }

        [Test]
        public async Task Should_be_healthy_with_configured_receive_endpoints()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILoggerFactory>(_ => LoggerFactory);
            collection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            collection.AddMassTransit(configurator =>
                {
                    configurator.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.UseMessageRetry(r => r.Immediate(5));
                        });
                    });
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
                await WaitForHealthStatus(healthChecks, HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
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

        public string FormatHealthCheck(HealthReport result)
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
