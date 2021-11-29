namespace MassTransit.Containers.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class EndpointConfiguration_Specs :
        BusTestFixture
    {
        [Test]
        public void Should_properly_configure_the_prefetch_count()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.PrefetchCount = 427;
            });

            var probe = JObject.Parse(busControl.GetProbeResult().ToJsonString());

            var prefetchCount = GetPrefetchCount(probe, 0);

            Assert.That(prefetchCount, Is.EqualTo(427));
        }

        [Test]
        public void Should_use_bus_setting_if_not_specified()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.PrefetchCount = 427;

                cfg.ReceiveEndpoint("input-queue", e =>
                {
                });
            });

            var probe = JObject.Parse(busControl.GetProbeResult().ToJsonString());

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(427));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));
        }

        [Test]
        public void Should_override_bus_setting_if_specified()
        {
            var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.PrefetchCount = 427;

                cfg.ReceiveEndpoint("input-queue", e =>
                {
                    e.PrefetchCount = 351;
                });
            });

            var jsonString = busControl.GetProbeResult().ToJsonString();
            var probe = JObject.Parse(jsonString);

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(351));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));
        }

        [Test]
        public async Task Should_include_concurrency_filter_if_specified()
        {
            var services = new ServiceCollection();
            services.AddSingleton(LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PingConsumer, EndpointPingConsumerDefinition>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.PrefetchCount = 427;

                    cfg.ConfigureEndpoints(context);
                });
            });
            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            var jsonString = busControl.GetProbeResult().ToJsonString();
            var probe = JObject.Parse(jsonString);

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(351));
            Assert.That(GetConcurrentMessageLimit(probe, 0), Is.EqualTo(100));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));

            await provider.DisposeAsync();
        }

        [Test]
        public async Task Should_include_concurrency_filter_if_concurrency_limit_specified()
        {
            var services = new ServiceCollection();
            services.AddSingleton(LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PingConsumer, PingConsumerDefinition>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.PrefetchCount = 427;

                    cfg.ConfigureEndpoints(context);
                });
            });
            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            var jsonString = busControl.GetProbeResult().ToJsonString();
            var probe = JObject.Parse(jsonString);

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(120));
            Assert.That(GetConcurrentMessageLimit(probe, 0), Is.EqualTo(100));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));

            await provider.DisposeAsync();
        }

        [Test]
        public async Task Should_include_concurrency_filter_if_concurrency_limit_overridden()
        {
            var services = new ServiceCollection();
            services.AddSingleton(LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PingConsumer>()
                    .Endpoint(e => e.ConcurrentMessageLimit = 100);

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.PrefetchCount = 427;

                    cfg.ConfigureEndpoints(context);
                });
            });
            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            var jsonString = busControl.GetProbeResult().ToJsonString();
            var probe = JObject.Parse(jsonString);

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(120));
            Assert.That(GetConcurrentMessageLimit(probe, 0), Is.EqualTo(100));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));

            await provider.DisposeAsync();
        }

        [Test]
        public async Task Should_include_nothing_if_not_specified()
        {
            var services = new ServiceCollection();
            services.AddSingleton(LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PingConsumer, EmptyPingConsumerDefinition>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.PrefetchCount = 427;

                    cfg.ConfigureEndpoints(context);
                });
            });
            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            var jsonString = busControl.GetProbeResult().ToJsonString();
            var probe = JObject.Parse(jsonString);

            Assert.That(GetPrefetchCount(probe, 0), Is.EqualTo(427));
            Assert.That(GetPrefetchCount(probe, 1), Is.EqualTo(427));

            await provider.DisposeAsync();
        }


        class PingConsumerDefinition :
            ConsumerDefinition<PingConsumer>
        {
            public PingConsumerDefinition()
            {
                ConcurrentMessageLimit = 100;
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<PingConsumer> consumerConfigurator)
            {
            }
        }


        class EndpointPingConsumerDefinition :
            ConsumerDefinition<PingConsumer>
        {
            public EndpointPingConsumerDefinition()
            {
                Endpoint(x =>
                {
                    x.PrefetchCount = 351;
                    x.ConcurrentMessageLimit = 100;
                });
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<PingConsumer> consumerConfigurator)
            {
            }
        }


        class EmptyPingConsumerDefinition :
            ConsumerDefinition<PingConsumer>
        {
            public EmptyPingConsumerDefinition()
            {
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<PingConsumer> consumerConfigurator)
            {
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        static int GetPrefetchCount(JObject jObject, int index)
        {
            var receiveEndpoints = jObject["results"]["bus"]["host"]["receiveEndpoint"];
            if (receiveEndpoints.Type == JTokenType.Array)
            {
                if (index < receiveEndpoints.Count())
                {
                    return receiveEndpoints[index]["receiveTransport"]["prefetchCount"].ToObject<int>();
                }
            }
            else if (receiveEndpoints.Type == JTokenType.Object)
            {
                if (index <= 0)
                    return receiveEndpoints["receiveTransport"]["prefetchCount"].ToObject<int>();
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        static int GetConcurrentMessageLimit(JObject jObject, int index)
        {
            var receiveEndpoints = jObject["results"]["bus"]["host"]["receiveEndpoint"];
            if (receiveEndpoints.Type == JTokenType.Array)
            {
                if (index < receiveEndpoints.Count())
                {
                    return receiveEndpoints[index]["receiveTransport"]["concurrentMessageLimit"].ToObject<int>();
                }
            }
            else if (receiveEndpoints.Type == JTokenType.Object)
            {
                if (index <= 0)
                    return receiveEndpoints["receiveTransport"]["concurrentMessageLimit"].ToObject<int>();
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        static int GetConcurrencyLimit(JObject jObject, int index)
        {
            var receiveEndpoints = jObject["results"]["bus"]["host"]["receiveEndpoint"];
            if (receiveEndpoints.Type == JTokenType.Array)
            {
                if (index < receiveEndpoints.Count())
                {
                    return receiveEndpoints[index]["filters"][2]["consumePipe"]["filters"]["filters"]["limit"].ToObject<int>();
                }
            }
            else if (receiveEndpoints.Type == JTokenType.Object)
            {
                if (index <= 0)
                    return receiveEndpoints["filters"][2]["consumePipe"]["filters"]["filters"]["limit"].ToObject<int>();
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public EndpointConfiguration_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
