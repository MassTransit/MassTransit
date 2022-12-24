namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public interface IMessageLatencyTransport :
        IAsyncDisposable
    {
        Task Send(LatencyTestMessage message);

        /// <summary>
        /// The bus control
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="reportConsumerMetric"></param>
        Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric);
    }
}
