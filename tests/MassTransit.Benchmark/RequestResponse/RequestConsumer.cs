namespace MassTransitBenchmark.RequestResponse
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class RequestConsumer :
        IConsumer<RequestMessage>
    {
        public static int CurrentConsumerCount;
        public static int MaxConsumerCount;
        readonly IReportConsumerMetric _report;

        public RequestConsumer(IReportConsumerMetric report)
        {
            _report = report;
        }

        public async Task Consume(ConsumeContext<RequestMessage> context)
        {
            var current = Interlocked.Increment(ref CurrentConsumerCount);
            var maxConsumerCount = MaxConsumerCount;
            if (current > maxConsumerCount)
                Interlocked.CompareExchange(ref MaxConsumerCount, current, maxConsumerCount);

            try
            {
                context.Respond(new ResponseMessage(context.Message.CorrelationId));

                await _report.Consumed<RequestMessage>(context.Message.CorrelationId).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref CurrentConsumerCount);
            }
        }
    }
}