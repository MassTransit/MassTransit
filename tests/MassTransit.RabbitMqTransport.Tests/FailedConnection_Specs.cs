namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Explicit]
    public class Failing_to_connect_to_rabbitmq :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_fault_nicely()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

                x.Host(new Uri("rabbitmq://unknownhost:32787"), h =>
                {
                    h.Username("whocares");
                    h.Password("Ohcrud");
                    h.RequestedConnectionTimeout(2000);
                });

                x.AutoStart = true;
            });

            Assert.ThrowsAsync<RabbitMqConnectionException>(async () =>
            {
                BusHandle handle;
                using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    handle = await busControl.StartAsync(timeout.Token).OrCanceled(TestCancellationToken);
                }

                await handle.StopAsync(CancellationToken.None);
            });
        }

        [Test]
        public async Task Should_fault_when_credentials_are_bad()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guessed");
                });
            });

            Assert.That(async () =>
            {
                var handle = await busControl.StartAsync();
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
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input-queue", e =>
                {
                });
            });

            var handle = await busControl.StartAsync();
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;

                await Task.Delay(30000);
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        [Explicit]
        public async Task Should_start_without_any_configuration()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
            });

            var handle = await busControl.StartAsync(new CancellationTokenSource(5000).Token);
            try
            {
                await handle.Ready;
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
            var busControl = Bus.Factory.CreateUsingRabbitMq(x => BusTestFixture.ConfigureBusDiagnostics(x));

            BusHandle handle;
            using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                handle = await busControl.StartAsync(timeout.Token);
            }

            try
            {
                await handle.Ready;
            }
            finally
            {
                await handle.StopAsync(CancellationToken.None);
            }
        }

        [Test]
        [Explicit]
        public async Task Should_startup_and_shut_down_cleanly_with_an_endpoint()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                });

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<Test>(async context =>
                    {
                    });
                });
            });

            var handle = await busControl.StartAsync(TestCancellationToken);
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
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

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
