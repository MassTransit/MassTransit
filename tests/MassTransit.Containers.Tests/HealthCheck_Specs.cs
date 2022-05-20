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
            });

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();

            await healthChecks.WaitForHealthStatus(HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);

                var busControl = provider.GetRequiredService<IBusControl>();

                var endpointHandle = busControl.ConnectReceiveEndpoint("another-queue", x =>
                {
                });

                await endpointHandle.Ready;

                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);

                using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await endpointHandle.ReceiveEndpoint.Stop(stop.Token);

                await healthChecks.WaitForHealthStatus(HealthStatus.Degraded);
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
            });

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();

            await healthChecks.WaitForHealthStatus(HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);

                var busControl = provider.GetRequiredService<IBusControl>();

                using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await busControl.StopAsync(stop.Token);

                await healthChecks.WaitForHealthStatus(HealthStatus.Unhealthy);

                using var start = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await busControl.StartAsync(start.Token);

                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);
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
            });
            collection.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(10);
                    options.StopTimeout = TimeSpan.FromSeconds(30);
                });

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();

            var result = await healthChecks.CheckHealthAsync(TestCancellationToken);
            Assert.That(result.Status == HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }

        [Test]
        public async Task Should_be_healthy_with_multiple_bus_instances()
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
            });
            collection.AddMassTransit<IAnotherBus>(configurator =>
            {
                configurator.UsingInMemory((context, cfg) =>
                {
                    cfg.AutoStart = true;
                });
            });

            IServiceProvider provider = collection.BuildServiceProvider(true);

            var healthChecks = provider.GetService<HealthCheckService>();

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();

            var result = await healthChecks.CheckHealthAsync(TestCancellationToken);
            Assert.That(result.Status == HealthStatus.Unhealthy);

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));
            try
            {
                await healthChecks.WaitForHealthStatus(HealthStatus.Healthy);
            }
            finally
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Task.WhenAll(hostedServices.Select(x => x.StopAsync(cts.Token)));
            }
        }


        public interface IAnotherBus :
            IBus
        {
        }
    }
}
