namespace MassTransit.Tests.Performance
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Performance_of_the_in_memory_transport
    {
        [Test]
        public async Task Should_be_wicked_fast()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.UseBsonSerializer();

                cfg.ReceiveEndpoint("input-queue", e =>
                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                    }));
            });

            await bus.StartAsync();
            try
            {
                var client = new MessageRequestClient<PingMessage, PongMessage>(bus, new Uri("loopback://localhost/input-queue"), TimeSpan.FromSeconds(30));

                int limit = 50000;
                int count = 0;

                await client.Request(new PingMessage());

                Stopwatch timer = Stopwatch.StartNew();

                await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
                {
                    await client.Request(new PingMessage());

                    Interlocked.Increment(ref count);
                }));

                timer.Stop();

                Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
                Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }


    [TestFixture, Explicit]
    public class Performance_of_dynamic_interface_implementations :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_also_really_fast()
        {
            int limit = 50000;
            int count = 0;

            await _requestClient.Request(new PerformanceRequestImpl());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PerformanceRequestImpl());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }

        IRequestClient<PerformanceRequest, PerformanceResult> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PerformanceRequest, PerformanceResult>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PerformanceRequest>(async context =>
            {
                await context.RespondAsync(new PerformanceResultImpl(context.Message.Id));
            });
        }


        public interface PerformanceResult
        {
            Guid Id { get; }
        }


        class PerformanceResultImpl :
            PerformanceResult
        {
            public PerformanceResultImpl(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; private set; }
        }


        public interface PerformanceRequest
        {
            Guid Id { get; }
        }


        class PerformanceRequestImpl :
            PerformanceRequest
        {
            public PerformanceRequestImpl()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; private set; }
        }
    }
}
