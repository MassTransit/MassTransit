namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using MultiBus;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
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

        protected static void ConfigureOne(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer1>();
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-one/"));
                cfg.ConfigureEndpoints(context);
                cfg.UseHealthCheck(context);
            });
        }

        protected static void ConfigureTwo(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer2>();
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-two/"));
                cfg.ConfigureEndpoints(context);
                cfg.UseHealthCheck(context);
            });
        }

        protected Common_MultiBus()
        {
            Task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            Task2 = GetTask<ConsumeContext<PingMessage>>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StartAsync(TestCancellationToken)));

            await WaitForHealthStatus(HealthCheckService, HealthStatus.Healthy);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StopAsync(TestCancellationToken)));
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
            {
                await TestContext.Out.WriteLineAsync(FormatHealthCheck(result));
            }

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
