#nullable enable
namespace MassTransit.AmazonSqsTransport.Tests.Persistence
{
    public class SimpleMessage
    {
        public MessageData<string>? BigData { get; set; }
    }
}
