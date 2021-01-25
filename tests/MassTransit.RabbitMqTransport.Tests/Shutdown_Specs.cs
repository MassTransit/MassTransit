namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Stopping_the_bus :
        BusTestFixture
    {
        [Test]
        [Category("Flaky")]
        public async Task Should_complete_running_consumers_nicely()
        {
            TaskCompletionSource<PingMessage> consumerStarted = GetTask<PingMessage>();

            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host("localhost", "test", h =>
                {
                });

                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync("Starting handler");

                        consumerStarted.TrySetResult(context.Message);

                        for (var i = 0; i < 5; i++)
                        {
                            await Task.Delay(1000);

                            await Console.Out.WriteLineAsync("Handler processing");
                        }

                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));

                        await Console.Out.WriteLineAsync("Handler complete");
                    });
                });
            });

            await Console.Out.WriteLineAsync("Starting bus");

            await bus.StartAsync(TestCancellationToken);

            await Console.Out.WriteLineAsync("Bus started");

            try
            {
                await bus.Publish(new PingMessage(), x =>
                {
                    x.RequestId = NewId.NextGuid();
                    x.ResponseAddress = bus.Address;
                });

                await consumerStarted.Task;

                await Console.Out.WriteLineAsync("Consumer Start Acknowledged");
            }
            finally
            {
                await Console.Out.WriteLineAsync("Stopping bus");

                await bus.StopAsync(TestCancellationToken);

                await Console.Out.WriteLineAsync("Bus stopped");
            }
        }

        [Test]
        [Category("Flaky")]
        public async Task Should_complete_with_nothing_running()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host("localhost", "test", h =>
                {
                });

                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync("Starting handler");

                        for (var i = 0; i < 5; i++)
                        {
                            await Task.Delay(1000);

                            await Console.Out.WriteLineAsync("Handler processing");
                        }

                        await Console.Out.WriteLineAsync("Handler complete");
                    });
                });
            });

            await Console.Out.WriteLineAsync("Starting bus");

            await bus.StartAsync(TestCancellationToken);

            await Console.Out.WriteLineAsync("Bus started");

            try
            {
                await Task.Delay(1000);
            }
            finally
            {
                await Console.Out.WriteLineAsync("Stopping bus");

                await bus.StopAsync(TestCancellationToken);

                await Console.Out.WriteLineAsync("Bus stopped");
            }
        }

        public Stopping_the_bus()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
