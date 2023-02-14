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
        Add<ushort>("kc|keys:", "The number of keys for the topic", x => KeysCount = x);
        Add<ushort>("cc|consumers:", "The number of concurrent kafka consumers", x => ConcurrentConsumerLimit = x);
        Add<string>("t|topic:", "The name of the topic to use", x => Topic = x);

        Host = "localhost:9092";
        Topic = "benchmark-topic";
        PartitionCount = 2;
        KeysCount = 20;
        ConcurrentConsumerLimit = 1;
    }

    public string Host { get; set; }
    public string Topic { get; set; }
    public ushort PartitionCount { get; set; }
    public ushort KeysCount { get; set; }
    public ushort ConcurrentConsumerLimit { get; set; }

    public void ShowOptions()
    {
        Console.WriteLine("Host: {0}", Host);
        Console.WriteLine("Topic: {0}", Topic);
        Console.WriteLine("Partitions Count: {0}", PartitionCount);
        Console.WriteLine("Keys Count: {0}", KeysCount);
        Console.WriteLine("Concurrent Consumers: {0}", ConcurrentConsumerLimit);
    }
}
