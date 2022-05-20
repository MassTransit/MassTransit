namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    public class Using_a_consumer_filter
    {
        static int _attempts;
        static int _lastAttempt;
        static int _lastCount;

        [Test]
        public async Task Should_produce()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<EventHubMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.UsingEventHub((context, k) =>
                        {
                            k.Host(Configuration.EventHubNamespace);
                            k.Storage(Configuration.StorageAccount);

                            k.ReceiveEndpoint(Configuration.EventHubName, c =>
                            {
                                c.UseMessageRetry(retry => retry.Immediate(3));

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var producer = await harness.GetProducer(Configuration.EventHubName);

            var messageId = NewId.NextGuid();

            await producer.Produce<EventHubMessage>(new { Text = "text" }, Pipe.Execute<SendContext>(context =>
            {
                context.MessageId = messageId;
            }), harness.CancellationToken);

            ConsumeContext<EventHubMessage> result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<EventHubMessage>>>().Task;

            Assert.That(_attempts, Is.EqualTo(4));
            Assert.That(_lastCount, Is.EqualTo(2));
            Assert.That(_lastAttempt, Is.EqualTo(3));
        }


        class KafkaMessageConsumer :
            IConsumer<EventHubMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<EventHubMessage> context)
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();
                _lastCount = context.GetRetryCount();

                TestContext.Out.WriteLine($"Attempt: {context.GetRetryAttempt()}");

                if (_lastCount < 2)
                    throw new Exception("Big bad exception");

                _taskCompletionSource.TrySetResult(context);

                return Task.CompletedTask;
            }
        }
    }
}
