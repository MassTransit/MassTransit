namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework.Messages;


    [TestFixture]
    [Explicit]
    public class Pounding_the_crap_out_of_the_send_endpoint :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_end_well()
        {
            var timer = Stopwatch.StartNew();

            var publishers = new Task[100 * 1000];
            Parallel.For(0, 100, i =>
            {
                var offset = i * 1000;

                for (var j = 0; j < 1000; j++)
                {
                    var ping = new PingMessage();
                    var task = Bus.Publish(ping);
                    publishers[offset + j] = task;
                }
            });

            var published = timer.Elapsed;

            await Task.WhenAll(publishers);

            var confirmed = timer.Elapsed;

            Console.WriteLine("Published {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, published.TotalMilliseconds,
                100L * 1000L * 1000L / published.TotalMilliseconds);

            Console.WriteLine("Confirmed {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, confirmed.TotalMilliseconds,
                100L * 1000L * 1000L / confirmed.TotalMilliseconds);

            await _completed.Task;

            var completed = timer.Elapsed;

            Console.WriteLine("Completed {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, completed.TotalMilliseconds,
                100L * 1000L * 1000L / completed.TotalMilliseconds);
        }

        TaskCompletionSource<int> _completed;

        public Pounding_the_crap_out_of_the_send_endpoint()
        {
            TestTimeout = TimeSpan.FromSeconds(180);
        }

        protected override void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.PublisherConfirmation = false;
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            _completed = GetTask<int>();

            configurator.ReceiveEndpoint("input_queue_express", x =>
            {
                x.AutoDelete = true;
                x.Durable = false;
                x.PrefetchCount = 10000;

                var count = 0;

                x.UseConcurrencyLimit(32);

                x.Handler<PingMessage>(async context =>
                {
                    if (Interlocked.Increment(ref count) == 100000)
                        _completed.TrySetResult(count);
                });
            });
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            model.ExchangeDelete("input_queue_express");
            model.QueueDelete("input_queue_express");
        }
    }
}
