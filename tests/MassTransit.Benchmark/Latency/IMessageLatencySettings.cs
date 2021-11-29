namespace MassTransitBenchmark.Latency
{
    public interface IMessageLatencySettings
    {
        long MessageCount { get; }

        int ConcurrencyLimit { get; }

        ushort PrefetchCount { get; }

        bool Durable { get; }

        int Clients { get; }

        int PayloadSize { get; }
    }
}