namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;


    public interface IReportConsumerMetric
    {
        Task Consumed<T>(Guid messageId)
            where T : class;

        Task<T> ResponseReceived<T>(Guid messageId, Task<T> requestTask)
            where T : class;
    }
}