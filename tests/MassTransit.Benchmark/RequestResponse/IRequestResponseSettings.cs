namespace MassTransitBenchmark.RequestResponse
{
    using System;


    public interface IRequestResponseSettings
    {
        long MessageCount { get; }

        int ConcurrencyLimit { get; }

        ushort PrefetchCount { get; }

        bool Durable { get; }

        int Clients { get; }

        TimeSpan RequestTimeout { get; }
    }
}