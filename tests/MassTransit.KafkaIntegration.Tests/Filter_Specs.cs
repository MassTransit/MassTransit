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
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
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

            await producer.Produce(new { }, harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.Multiple(() =>
            {
                Assert.That(_attempts, Is.EqualTo(4));
                Assert.That(_lastCount, Is.EqualTo(2));
                Assert.That(_lastAttempt, Is.EqualTo(3));
            });
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
        }
    }


    [TestFixture]
    public class Using_a_scoped_send_filter
    {
        [Test]
        public async Task Should_properly_configure_the_scoped_filter()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<ScopedContext>()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.UseSendFilter(typeof(ScopedContextSendFilter<>), context);

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Using_a_scoped_send_filter), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
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

            await producer.Produce(new { }, harness.CancellationToken);

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.That(result.Headers.Get<string>("Scoped-Value"), Is.EqualTo("Hello, World"));
        }

        const string Topic = "scoped-filter-producer";


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


        public record KafkaMessage
        {
        }
    }


    [TestFixture]
    public class Using_a_multiple_scoped_send_filters
    {
        [Test]
        public async Task Should_properly_configure_the_scoped_filter()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<ScopedContext>()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.UseSendFilter(typeof(ScopedContextSendFilter<>), context);

                            k.UseSendFilter(typeof(SecondScopedContextSendFilter<>), context);

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Using_a_scoped_send_filter), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var scopedContext = harness.Scope.ServiceProvider.GetRequiredService<ScopedContext>();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { }, harness.CancellationToken);

            await provider.GetTask<ConsumeContext<KafkaMessage>>();

            await scopedContext.SecondTask.Task;

            await scopedContext.FirstTask.Task;
        }

        const string Topic = "scoped-filters-producer";


        public class ScopedContext
        {
            public ScopedContext(ITestHarness harness)
            {
                FirstTask = harness.GetTask<SendContext>();
                SecondTask = harness.GetTask<SendContext>();
            }

            public TaskCompletionSource<SendContext> FirstTask { get; set; }
            public TaskCompletionSource<SendContext> SecondTask { get; set; }
        }


        public class ScopedContextSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly ScopedContext _scopedContext;

            public ScopedContextSendFilter(ScopedContext scopedContext)
            {
                _scopedContext = scopedContext;
            }

            public void Probe(ProbeContext context)
            {
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                _scopedContext.FirstTask.TrySetResult(context);

                return next.Send(context);
            }
        }


        public class SecondScopedContextSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly ScopedContext _scopedContext;

            public SecondScopedContextSendFilter(ScopedContext scopedContext)
            {
                _scopedContext = scopedContext;
            }

            public void Probe(ProbeContext context)
            {
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                _scopedContext.SecondTask.TrySetResult(context);

                return next.Send(context);
            }
        }


        public record KafkaMessage
        {
        }
    }
}
