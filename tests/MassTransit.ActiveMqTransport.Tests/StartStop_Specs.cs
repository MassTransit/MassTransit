namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    [Category("Flaky")]
    public class StartStop_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingActiveMq(x =>
            {
                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage());
                    });
                });
            });

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }
        }

        public StartStop_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class Restart_should_not_lose_messages :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            int count = 0;

            var bus = MassTransit.Bus.Factory.CreateUsingActiveMq(x =>
            {
                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.ConcurrentMessageLimit = 3;
                    e.PrefetchCount = 10;

                    e.Handler<SuperMessage>(async context =>
                    {
                        await Task.Delay(5000);

                        Interlocked.Increment(ref count);
                    });
                });
            });

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.PublishBatch(Enumerable.Range(0, 30).Select(x => new SuperMessage() { Value = x.ToString() }));

                await Task.Delay(15000);
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await Task.Delay(40000);
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            Assert.That(count, Is.EqualTo(30));
        }


        public class SuperMessage
        {
            public string Value { get; set; }
        }


        public Restart_should_not_lose_messages()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
