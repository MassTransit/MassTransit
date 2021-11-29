namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Util;


    /// <summary>
    /// Benchmark that determines the latency of messages between the time the message is published
    /// to the broker until it is acked by RabbitMQ. And then consumed by the message consumer.
    /// </summary>
    public class RequestResponseBenchmark
    {
        readonly IRequestResponseSettings _settings;
        readonly IRequestResponseTransport _transport;
        MessageMetricCapture _capture;
        TimeSpan _consumeDuration;
        TimeSpan _requestDuration;

        public RequestResponseBenchmark(IRequestResponseTransport transport, IRequestResponseSettings settings)
        {
            _transport = transport;
            _settings = settings;

            if (settings.MessageCount / settings.Clients * settings.Clients != settings.MessageCount)
            {
                throw new ArgumentException("The clients must be a factor of message count");
            }
        }

        public void Run(CancellationToken cancellationToken = default)
        {
            _capture = new MessageMetricCapture(_settings.MessageCount);

            _transport.GetBusControl(ConfigureReceiveEndpoint);
            try
            {
                Console.WriteLine("Running Request Response Benchmark");

                TaskUtil.Await(RunBenchmark, cancellationToken);

                Console.WriteLine("Message Count: {0}", _settings.MessageCount);
                Console.WriteLine("Clients: {0}", _settings.Clients);
                Console.WriteLine("Durable: {0}", _settings.Durable);
                Console.WriteLine("Prefetch Count: {0}", _settings.PrefetchCount);
                Console.WriteLine("Concurrency Limit: {0}", _settings.ConcurrencyLimit);

                Console.WriteLine("Total consume duration: {0:g}", _consumeDuration);
                Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
                    _settings.MessageCount * 1000 / _consumeDuration.TotalMilliseconds);
                Console.WriteLine("Total request duration: {0:g}", _requestDuration);
                Console.WriteLine("Request rate: {0:F2} (msg/s)",
                    _settings.MessageCount * 1000 / _requestDuration.TotalMilliseconds);
                Console.WriteLine("Concurrent Consumer Count: {0}", RequestConsumer.MaxConsumerCount);

                MessageMetric[] messageMetrics = _capture.GetMessageMetrics();

                Console.WriteLine("Avg Request Time: {0:F0}ms",
                    messageMetrics.Average(x => x.RequestLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Min Request Time: {0:F0}ms",
                    messageMetrics.Min(x => x.RequestLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Max Request Time: {0:F0}ms",
                    messageMetrics.Max(x => x.RequestLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("Med Request Time: {0:F0}ms",
                    messageMetrics.Median(x => x.RequestLatency) * 1000 / Stopwatch.Frequency);
                Console.WriteLine("95t Request Time: {0:F0}ms",
                    messageMetrics.Percentile(x => x.RequestLatency) * 1000 / Stopwatch.Frequency);

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

                Console.WriteLine("Request duration distribution");

                DrawResponseTimeGraph(messageMetrics, x => x.RequestLatency);
            }
            finally
            {
                _transport.Dispose();
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
                select new {Value = segment.Key, Count = segment.Count()}).ToList();

            var maxCount = histogram.Max(x => x.Count);

            foreach (var item in histogram)
            {
                var barLength = item.Count * 60 / maxCount;
                Console.WriteLine("{0,5}ms {2,-60} ({1,7})", (minTime + increment * item.Value) * 1000 / Stopwatch.Frequency, item.Count,
                    new string('*', barLength));
            }
        }

        async Task RunBenchmark()
        {
            await Task.Yield();

            var stripes = new Task[_settings.Clients];

            for (var i = 0; i < _settings.Clients; i++)
            {
                var requestClient = await _transport.GetRequestClient<RequestMessage>(_settings.RequestTimeout).ConfigureAwait(false);

                stripes[i] = RunStripe(requestClient, _settings.MessageCount / _settings.Clients);
            }

            await Task.WhenAll(stripes).ConfigureAwait(false);

            _requestDuration = await _capture.RequestCompleted.ConfigureAwait(false);
            _consumeDuration = await _capture.ConsumeCompleted.ConfigureAwait(false);
        }

        async Task RunStripe(IRequestClient<RequestMessage> client, long messageCount)
        {
            await Task.Yield();

            for (long i = 0; i < messageCount; i++)
            {
                var messageId = NewId.NextGuid();
                var task = client.GetResponse<ResponseMessage>(new RequestMessage(messageId));

                await _capture.ResponseReceived(messageId, task).ConfigureAwait(false);
            }
        }

        void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            if (_settings.ConcurrencyLimit > 0)
                configurator.UseConcurrencyLimit(_settings.ConcurrencyLimit);

            configurator.Consumer(() => new RequestConsumer(_capture));
        }
    }
}
