namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using NDesk.Options;


    public class RequestResponseOptionSet :
        OptionSet,
        IRequestResponseSettings
    {
        public RequestResponseOptionSet()
        {
            Add<long>("count:", "The number of messages to send", value => MessageCount = value);
            Add<ushort>("prefetch:", "The prefetch count for the broker", value => PrefetchCount = value);
            Add<int>("concurrency:", "The number of concurrent consumers", value => ConcurrencyLimit = value);
            Add<int>("clients:", "The number of sending message clients", value => Clients = value);
            Add<bool>("durable:", "The number of concurrent consumers", value => Durable = value);
            Add<int>("timeout:", "The request timeout, in seconds", value => RequestTimeout = TimeSpan.FromSeconds(value));

            MessageCount = 10000;
            PrefetchCount = 100;
            ConcurrencyLimit = 0;
            Clients = 10;
            Durable = false;
            RequestTimeout = TimeSpan.FromSeconds(30);
        }

        public bool Durable { get; set; }
        public long MessageCount { get; set; }
        public ushort PrefetchCount { get; set; }
        public int Clients { get; set; }
        public int ConcurrencyLimit { get; set; }
        public TimeSpan RequestTimeout { get; set; }
    }
}