namespace MassTransitBenchmark;

using System;
using NDesk.Options;


public class KafkaOptionSet :
    OptionSet
{
    public KafkaOptionSet()
    {
        Add<string>("h|host:", "The host name of the broker", x => Host = x);
        Add<ushort>("pc|partitions:", "The number of partitions for the topic", x => PartitionCount = x);
        Add<string>("t|topic:", "The name of the topic to use", x => Topic = x);

        Host = "localhost:9092";
        Topic = "benchmark-topic";
        PartitionCount = 2;
    }

    public string Host { get; set; }
    public string Topic { get; set; }
    public ushort PartitionCount { get; set; }

    public void ShowOptions()
    {
        Console.WriteLine("Host: {0}", Host);
    }
}
