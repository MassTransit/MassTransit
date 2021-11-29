namespace MassTransitBenchmark.Latency
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class MessageLatencyConsumer :
        IConsumer<LatencyTestMessage>
    {
        public static int CurrentConsumerCount;
        public static int MaxConsumerCount;
        readonly IReportConsumerMetric _report;

        public MessageLatencyConsumer(IReportConsumerMetric report)
        {
            _report = report;
        }

        public async Task Consume(ConsumeContext<LatencyTestMessage> context)
        {
            var current = Interlocked.Increment(ref CurrentConsumerCount);
            var maxConsumerCount = MaxConsumerCount;
            if (current > maxConsumerCount)
                Interlocked.CompareExchange(ref MaxConsumerCount, current, maxConsumerCount);

            try
            {
                await _report.Consumed<LatencyTestMessage>(context.Message.CorrelationId).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref CurrentConsumerCount);
            }
        }
    }
}