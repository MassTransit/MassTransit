namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Testing;


    public class Using_a_consumer_filter
    {
        const string Topic = "filter";

        static int _attempts;
        static int _lastAttempt;
        static int _lastCount;

        [Test]
        public async Task Should_properly_configure_the_filter()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.Host("localhost:9092");

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();

                                c.UseMessageRetry(retry => retry.Immediate(3));

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            var messageId = NewId.NextGuid();

            await producer.Produce(new { Text = "text" }, Pipe.Execute<SendContext>(context =>
            {
                context.MessageId = messageId;
            }), harness.CancellationToken);

            ConsumeContext<KafkaMessage> result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<KafkaMessage>>>().Task;

            Assert.That(_attempts, Is.EqualTo(4));
            Assert.That(_lastCount, Is.EqualTo(2));
            Assert.That(_lastAttempt, Is.EqualTo(3));
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<KafkaMessage> context)
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


        public interface KafkaMessage
        {
            string Text { get; }
        }
    }


    [TestFixture]
    public class Using_a_scoped_send_filter
    {
        const string Topic = "scoped-filter-producer";

        [Test]
        public async Task Should_properly_configure_the_filter()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<ScopedContext>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.Host("localhost:9092");

                            k.UseSendFilter(typeof(ScopedContextSendFilter<>), context);

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Using_a_scoped_send_filter), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var scopedContext = harness.Scope.ServiceProvider.GetRequiredService<ScopedContext>();

            scopedContext.Value = "Hello, World";

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { Text = "text" }, harness.CancellationToken);

            var result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<KafkaMessage>>>().Task;

            Assert.That(result.Headers.Get<string>("Scoped-Value"), Is.EqualTo("Hello, World"));
        }

        public class ScopedContext
        {
            public string Value { get; set; }
        }


        public class ScopedContextSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly ILogger<ScopedContext> _logger;
            readonly ScopedContext _scopedContext;

            public ScopedContextSendFilter(ScopedContext scopedContext, ILogger<ScopedContext> logger)
            {
                _logger = logger;
                _scopedContext = scopedContext;
            }

            public void Probe(ProbeContext context)
            {
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                _logger.LogInformation("Send Scoped Filter: {Value}", _scopedContext.Value);

                context.Headers.Set("Scoped-Value", _scopedContext.Value);

                return next.Send(context);
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);

                return Task.CompletedTask;
            }
        }


        public record KafkaMessage
        {
            public string Text { get; init; }
        }
    }
}
