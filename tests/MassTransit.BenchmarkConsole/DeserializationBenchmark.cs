namespace MassTransit.BenchmarkConsole;

using System;
using BenchmarkDotNet.Attributes;
using Context;
using Serialization;


[MemoryDiagnoser]
public class DeserializationBenchmark
{
    readonly MessagePackMessageSerializer _messagepackSerializer;
    readonly SystemTextJsonMessageSerializer _systemTextJsonSerializer;

    MessageBody _messagePackMessageBody;
    MessageBody _systemTextJsonMessageBody;

    [Params(0, 4096, 16384)]
    public int MessageBufferSize { get; set; }

    public DeserializationBenchmark()
    {
        _messagepackSerializer = new MessagePackMessageSerializer();
        _systemTextJsonSerializer = SystemTextJsonMessageSerializer.Instance;
    }

    [GlobalSetup]
    public void Setup()
    {
        var bufferContent = new byte[MessageBufferSize];
        Random.Shared.NextBytes(bufferContent);

        var initialMessage = new TypeToDeserialize
        {
            StringValue = "Hello, World!",
            IntValue = 42,
            GuidValue = Guid.NewGuid(),
            DateTimeValue = DateTime.UtcNow,
            ByteArrayValue = bufferContent
        };

        var sendContext = new MessageSendContext<TypeToDeserialize>(initialMessage);

        _messagePackMessageBody = _messagepackSerializer.GetMessageBody(sendContext);
        _systemTextJsonMessageBody = _systemTextJsonSerializer.GetMessageBody(sendContext);

        // Triggers any lazy serialization.
        _ = _messagePackMessageBody.GetBytes();
        _ = _systemTextJsonMessageBody.GetBytes();
    }

    [Benchmark]
    public void MessagePack_Deserialize()
    {
        var serializerContext = _messagepackSerializer.Deserialize(_messagePackMessageBody, null, null);
        _ = serializerContext.TryGetMessage<TypeToDeserialize>(out _);
    }

    [Benchmark(Baseline = true)]
    public void SystemTextJson_Deserialize()
    {
        var serializerContext = _systemTextJsonSerializer.Deserialize(_systemTextJsonMessageBody, null, null);
        _ = serializerContext.TryGetMessage<TypeToDeserialize>(out _);
    }

    class TypeToDeserialize
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public Guid GuidValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public byte[] ByteArrayValue { get; set; }
    }
}
