namespace MassTransit.BenchmarkConsole;

using System;
using BenchmarkDotNet.Attributes;
using Serialization;


[MemoryDiagnoser]
public class SerializationBenchmark
{
    readonly MessagePackMessageSerializer _messagepackSerializer;
    readonly SystemTextJsonMessageSerializer _systemTextJsonSerializer;
    TypeToSerialize _serializationSubject = null!;

    [Params(0, 4096)]
    public int MessageBufferSize { get; set; }

    public SerializationBenchmark()
    {
        _messagepackSerializer = new MessagePackMessageSerializer();
        _systemTextJsonSerializer = SystemTextJsonMessageSerializer.Instance;
    }

    [GlobalSetup]
    public void Setup()
    {
        var bufferContent = new byte[MessageBufferSize];
        Random.Shared.NextBytes(bufferContent);

        _serializationSubject = new TypeToSerialize
        {
            StringValue = "Hello, World!",
            IntValue = 42,
            GuidValue = Guid.NewGuid(),
            DateTimeValue = DateTime.UtcNow,
            ByteArrayValue = bufferContent
        };
    }

    [Benchmark]
    public void MessagePack_SerializeObject()
    {
        var messageBody = _messagepackSerializer.SerializeObject(_serializationSubject);

        _ = messageBody.GetBytes();
    }

    [Benchmark(Baseline = true)]
    public void SystemTextJson_SerializeObject()
    {
        var messageBody = _systemTextJsonSerializer.SerializeObject(_serializationSubject);

        _ = messageBody.GetBytes();
    }


    class TypeToSerialize
    {
        public required string StringValue { get; set; }
        public required int IntValue { get; set; }
        public required Guid GuidValue { get; set; }
        public required DateTime DateTimeValue { get; set; }
        public required byte[] ByteArrayValue { get; set; }
    }
}
