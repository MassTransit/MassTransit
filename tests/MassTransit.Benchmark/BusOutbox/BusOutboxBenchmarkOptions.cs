namespace MassTransitBenchmark.BusOutbox;

using NDesk.Options;


public class BusOutboxBenchmarkOptions :
    OptionSet
{
    public BusOutboxBenchmarkOptions()
    {
        Add<long>("count:", "The number of messages to send", value => MessageCount = value);
        Add<ushort>("prefetch:", "The prefetch count for the broker", value => PrefetchCount = value);
        Add<int>("concurrency:", "The number of concurrent consumers", value => ConcurrencyLimit = value);
        Add<int>("clients:", "The number of sending message clients", value => Clients = value);
        Add<int>("payload:", "The size of the additional payload for the message", value => PayloadSize = value);
        MessageCount = 10000;
        PrefetchCount = 100;
        Clients = 10;
    }

    public int PayloadSize { get; set; }
    public long MessageCount { get; set; }
    public ushort PrefetchCount { get; set; }
    public int Clients { get; set; }
    public int? ConcurrencyLimit { get; set; }
}
