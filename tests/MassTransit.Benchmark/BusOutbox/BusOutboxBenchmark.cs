namespace MassTransitBenchmark.BusOutbox
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Latency;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    /// <summary>
    /// Benchmark that determines the latency of messages between the time the message is published
    /// to the broker until it is acked by RabbitMQ. And then consumed by the message consumer.
    /// </summary>
    public class BusOutboxBenchmark
    {
        readonly BusOutboxBenchmarkOptions _options;
        readonly string _payload;
        readonly IConfigureBusOutboxTransport _transport;
        MessageMetricCapture _capture;
        TimeSpan _consumeDuration;
        TimeSpan _sendDuration;

        public BusOutboxBenchmark(IConfigureBusOutboxTransport transport, BusOutboxBenchmarkOptions options)
        {
            _transport = transport;
            _options = options;

            if (options.MessageCount / options.Clients * options.Clients != options.MessageCount)
                throw new ArgumentException("The clients must be a factor of message count");

            _payload = _options.PayloadSize > 0 ? new string('*', _options.PayloadSize) : null;
        }

        public async Task Run()
        {
            _capture = new MessageMetricCapture(_options.MessageCount);

            await using var provider = new ServiceCollection()
                .AddTextLogger(Console.Out)
                .AddHostedService<MigrationHostedService<BusOutboxDbContext>>()
                .AddSingleton<IReportConsumerMetric>(_capture)
                .AddSingleton<ISendObserver,MetricSendObserver>()
                .AddMassTransit(x =>
                {
                    x.AddConsumer<BusOutboxMessageConsumer>();

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddDbContext<BusOutboxDbContext>(db =>
                    {
                        db.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), options =>
                        {
                            options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            options.MigrationsHistoryTable($"__{nameof(BusOutboxDbContext)}");

                            options.MinBatchSize(1);
                        });
                    });

                    x.AddEntityFrameworkOutbox<BusOutboxDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromMilliseconds(10);

                        o.UseBusOutbox();
                    });

                    _transport.Using(x, (context, cfg) =>
                    {
                        cfg.PrefetchCount = _options.PrefetchCount;
                        cfg.ConcurrentMessageLimit = _options.ConcurrencyLimit;
                    });
                })
                .BuildServiceProvider(true);

            await provider.StartHostedServices();

            try
            {
                Console.WriteLine("Running Bus Outbox Benchmark");

                await RunBenchmark(provider);

                Console.WriteLine("Message Count: {0}", _options.MessageCount);
                Console.WriteLine("Clients: {0}", _options.Clients);
                Console.WriteLine("Payload Length: {0}", _payload?.Length ?? 0);
                Console.WriteLine("Prefetch Count: {0}", _options.PrefetchCount);
                Console.WriteLine("Concurrency Limit: {0}", _options.ConcurrencyLimit);

                Console.WriteLine("Total send duration: {0:g}", _sendDuration);
                Console.WriteLine("Send message rate: {0:F2} (msg/s)",
                    _options.MessageCount * 1000 / _sendDuration.TotalMilliseconds);
                Console.WriteLine("Total consume duration: {0:g}", _consumeDuration);
                Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
                    _options.MessageCount * 1000 / _consumeDuration.TotalMilliseconds);
                Console.WriteLine("Concurrent Consumer Count: {0}", MessageLatencyConsumer.MaxConsumerCount);

                MessageMetric[] messageMetrics = _capture.GetMessageMetrics();

                Console.WriteLine("Avg Ack Time: {0:F0}ms",
                    messageMetrics.Average(x => x.AckLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Min Ack Time: {0:F0}ms",
                    messageMetrics.Min(x => x.AckLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Max Ack Time: {0:F0}ms",
                    messageMetrics.Max(x => x.AckLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Med Ack Time: {0:F0}ms",
                    messageMetrics.Median(x => x.AckLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("95t Ack Time: {0:F0}ms",
                    messageMetrics.Percentile(x => x.AckLatency) * 1000 / Stopwatch.Frequency);

                Console.WriteLine("Avg Consume Time: {0:F0}ms",
                    messageMetrics.Average(x => x.ConsumeLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Min Consume Time: {0:F0}ms",
                    messageMetrics.Min(x => x.ConsumeLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Max Consume Time: {0:F0}ms",
                    messageMetrics.Max(x => x.ConsumeLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Med Consume Time: {0:F0}ms",
                    messageMetrics.Median(x => x.ConsumeLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("95t Consume Time: {0:F0}ms",
                    messageMetrics.Percentile(x => x.ConsumeLatency) * 1000 / Stopwatch.Frequency);

                Console.WriteLine();
                DrawResponseTimeGraph(messageMetrics, x => x.ConsumeLatency);
            }
            finally
            {
                await provider.StopHostedServices();
            }
        }

        void DrawResponseTimeGraph(MessageMetric[] metrics, Func<MessageMetric, long> selector)
        {
            var maxTime = metrics.Max(selector);
            var minTime = metrics.Min(selector);

            const int segments = 10;

            var span = maxTime - minTime;
            var increment = span / segments;

            var histogram = (from x in metrics.Select(selector)
                let key = (x - minTime) * segments / span
                where key >= 0 && key < segments
                let groupKey = key
                group x by groupKey
                into segment
                orderby segment.Key
                select new
                {
                    Value = segment.Key,
                    Count = segment.Count()
                }).ToList();

            var maxCount = histogram.Max(x => x.Count);

            foreach (var item in histogram)
            {
                var barLength = item.Count * 60 / maxCount;
                Console.WriteLine("{0,5}ms {2,-60} ({1,7})", (minTime + increment * item.Value) * 1000 / Stopwatch.Frequency, item.Count,
                    new string('*', barLength));
            }
        }

        async Task RunBenchmark(IServiceProvider provider)
        {
            var stripes = new Task[_options.Clients];

            for (var i = 0; i < _options.Clients; i++)
                stripes[i] = Task.Run(() => RunStripe(provider, _options.MessageCount / _options.Clients));

            await Task.WhenAll(stripes).ConfigureAwait(false);

            _sendDuration = await _capture.SendCompleted.ConfigureAwait(false);

            _consumeDuration = await _capture.ConsumeCompleted.ConfigureAwait(false);
        }

        async Task RunStripe(IServiceProvider provider, long messageCount)
        {
            var endpointNameFormatter = provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

            var address = new Uri($"queue:{endpointNameFormatter.Consumer<BusOutboxMessageConsumer>()}");

            for (long i = 0; i < messageCount; i++)
            {
                await using var scope = provider.CreateAsyncScope();

                await using var dbContext = scope.ServiceProvider.GetService<BusOutboxDbContext>();

                var messageId = NewId.NextGuid();
                var sendEndpoint = await scope.ServiceProvider.GetService<ISendEndpointProvider>().GetSendEndpoint(address);
                var task = sendEndpoint.Send(new BusOutboxMessage(messageId, _payload), x => x.MessageId = messageId);

                await _capture.Sent(messageId, task).ConfigureAwait(false);

                await dbContext.SaveChangesAsync();
            }
        }


        class MetricSendObserver :
            ISendObserver
        {
            readonly IReportConsumerMetric _metric;

            public MetricSendObserver(IReportConsumerMetric metric)
            {
                _metric = metric;
            }

            public Task PreSend<T>(SendContext<T> context)
                where T : class
            {
                return Task.CompletedTask;
            }

            public Task PostSend<T>(SendContext<T> context)
                where T : class
            {
                _metric.PostSend(context.MessageId.Value);

                return Task.CompletedTask;
            }

            public Task SendFault<T>(SendContext<T> context, Exception exception)
                where T : class
            {
                return Task.CompletedTask;
            }
        }
    }


    public static class BenchmarkServiceCollectionExtensions
    {
        public static IServiceCollection AddTextLogger(this IServiceCollection services, TextWriter textWriter)
        {
            services.AddOptions<TextWriterLoggerOptions>();
            services.TryAddSingleton<ILoggerFactory>(provider =>
                new TextWriterLoggerFactory(textWriter, provider.GetRequiredService<IOptions<TextWriterLoggerOptions>>()));
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddOptions<TextWriterLoggerOptions>().Configure(options =>
            {
                options.Disable("Microsoft");
                options.LogLevel = LogLevel.Information;
            });

            return services;
        }
    }
}
