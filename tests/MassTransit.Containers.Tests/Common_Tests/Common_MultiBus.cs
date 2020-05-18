namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using Registration;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public abstract class Common_MultiBus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive()
        {
            await One.Publish<SimpleMessageInterface>(new SimpleMessageClass("abc"));

            await Task1.Task;
            await Task2.Task;
        }

        protected readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> Task1;
        protected readonly TaskCompletionSource<ConsumeContext<PingMessage>> Task2;
        protected abstract IBusOne One { get; }
        protected abstract HealthCheckService HealthCheckService { get; }
        protected abstract IEnumerable<IHostedService> HostedServices { get; }

        protected static void ConfigureOne<T>(IRegistrationConfigurator<IBusOne, T> configurator)
            where T : class
        {
            configurator.AddConsumer<Consumer1>();
            configurator.AddBus(context => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.Host(new Uri("loopback://bus-one/"));
                cfg.ConfigureEndpoints(context);
                cfg.UseHealthCheck(context);
            }));
        }

        protected static void ConfigureTwo<T>(IRegistrationConfigurator<IBusTwo, T> configurator)
            where T : class
        {
            configurator.AddConsumer<Consumer2>();
            configurator.AddBus(context => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.Host(new Uri("loopback://bus-two/"));
                cfg.ConfigureEndpoints(context);
                cfg.UseHealthCheck(context);
            }));
        }

        protected Common_MultiBus()
        {
            Task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            Task2 = GetTask<ConsumeContext<PingMessage>>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var result = await HealthCheckService.CheckHealthAsync(TestCancellationToken);
            Assert.That(result.Status == HealthStatus.Unhealthy);

            await Task.WhenAll(HostedServices.Select(x => x.StartAsync(TestCancellationToken)));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            do
            {
                result = await HealthCheckService.CheckHealthAsync(TestCancellationToken);

                Console.WriteLine("Health Check: {0}",
                    string.Join(", ", result.Entries.Select(x => string.Join("=", x.Key, x.Value.Status))));

                await Task.Delay(100, TestCancellationToken);
            }
            while (result.Status == HealthStatus.Unhealthy);

            Assert.That(result.Status == HealthStatus.Healthy);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StopAsync(TestCancellationToken)));
        }


        class Consumer1 :
            IConsumer<SimpleMessageInterface>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _taskCompletionSource;

            public Consumer1(IPublishEndpoint publishEndpointDefault, IBusTwo publishEndpoint,
                TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish(new PingMessage());
            }
        }


        class Consumer2 :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _taskCompletionSource;

            public Consumer2(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface IBusOne :
            IBus
        {
        }


        public class BusOne :
            BusInstance<IBusOne>,
            IBusOne
        {
            public BusOne(IBusControl busControl)
                : base(busControl)
            {
            }
        }


        public interface IBusTwo :
            IBus
        {
        }
    }
}
