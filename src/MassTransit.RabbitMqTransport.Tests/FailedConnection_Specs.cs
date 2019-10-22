namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Logging;


    [TestFixture]
    [Explicit]
    public class Failing_to_connect_to_rabbitmq :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_fault_nicely()
        {
            var loggerFactory = new TestOutputLoggerFactory(true);

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());

            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://unknownhost:32787"), h =>
                {
                    h.Username("whocares");
                    h.Password("Ohcrud");
                    h.RequestedConnectionTimeout(2000);
                });

                x.AutoStart = false;
            });

            Assert.That(async () =>
            {
                BusHandle handle = await busControl.StartAsync(new CancellationTokenSource(20000).Token);
                try
                {
                    TestContext.Out.WriteLine("Waiting for connection...");

                    await handle.Ready;
                }
                finally
                {
                    await handle.StopAsync();
                }
            }, Throws.TypeOf<RabbitMqConnectionException>());
        }

        [Test]
        public async Task Should_fault_when_credentials_are_bad()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guessed");
                });
            });

            Assert.That(async () =>
            {
                BusHandle handle = await busControl.StartAsync();
                try
                {
                    Console.WriteLine("Waiting for connection...");

                    await handle.Ready;
                }
                finally
                {
                    await handle.StopAsync();
                }
            }, Throws.TypeOf<RabbitMqConnectionException>());
        }

        [Test]
        [Explicit]
        public async Task Should_recover_from_a_crashed_server()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                });
            });

            BusHandle handle = await busControl.StartAsync();
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;

                for (var i = 0; i < 30; i++)
                {
                    try
                    {
                        await Task.Delay(1000);

//                        await busControl.Publish(new TestMessage());

                        Console.WriteLine("Published: {0}", i);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Publish {0} faulted: {1}", i, ex.Message);
                    }
                }
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        [Explicit]
        public async Task Should_startup_and_shut_down_cleanly()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                });
            });

            BusHandle handle = await busControl.StartAsync();
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        [Explicit]
        public async Task Should_startup_and_shut_down_cleanly_with_an_endpoint()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.Handler<Test>(async context =>
                    {
                    });
                });
            });

            BusHandle handle = await busControl.StartAsync(TestCancellationToken);
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;

                await Task.Delay(60000);
            }
            finally
            {
                await handle.StopAsync(TestCancellationToken);
            }
        }

        [Test]
        [Explicit]
        public async Task Should_startup_and_shut_down_cleanly_with_publish()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                });
            });

            await busControl.StartAsync();
            try
            {
                await busControl.Publish(new TestMessage());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        public Failing_to_connect_to_rabbitmq()
            : base(new InMemoryTestHarness())
        {
        }


        public interface Test
        {
        }


        public class TestMessage : Test
        {
        }
    }
}
