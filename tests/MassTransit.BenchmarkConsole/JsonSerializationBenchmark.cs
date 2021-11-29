namespace MassTransit.BenchmarkConsole
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using Context;
    using Serialization;


    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class JsonSerializationBenchmark
    {
        readonly MessageSendContext<AverageMessage> _averageMessageSendContext;
        readonly NewtonsoftJsonMessageSerializer _newtonsoftJsonMessageSerializer;

        public JsonSerializationBenchmark()
        {
            _averageMessageSendContext = new MessageSendContext<AverageMessage>(new AverageMessage
            {
                CorrelationId = NewId.NextGuid(),
                Name = "Franklin",
                SomeValue = 27,
                SomeOptionalValue = 42,
                Created = DateTime.UtcNow,
                Amount = 123.45m
            })
            {
                DestinationAddress = new Uri("loopback://localhost/input-queue"),
                SourceAddress = new Uri("loopback://localhost/request-client-queue"),
                CorrelationId = NewId.NextGuid(),
                ConversationId = NewId.NextGuid(),
            };

            _averageMessageSendContext.Headers.Set("MT-Activity-Id", NewId.NextGuid().ToString());

            _newtonsoftJsonMessageSerializer = new NewtonsoftJsonMessageSerializer();
        }

        [Benchmark(Description = "System.Text.Json byte[]")]
        public void SystemTextJson()
        {
            var bytes = SystemTextJsonMessageSerializer.Instance.GetMessageBody(_averageMessageSendContext).GetBytes();
        }

        [Benchmark(Description = "System.Text.Json string")]
        public void SystemTextJsonString()
        {
            var text = SystemTextJsonMessageSerializer.Instance.GetMessageBody(_averageMessageSendContext).GetString();
        }

        [Benchmark(Description = "Newtonsoft.Json byte[]")]
        public void NewtonsoftJson()
        {
            var bytes = _newtonsoftJsonMessageSerializer.GetMessageBody(_averageMessageSendContext).GetBytes();
        }
    }


    public class AverageMessage
    {
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public int SomeValue { get; set; }
        public int? SomeOptionalValue { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public decimal Amount { get; set; }
    }
}
