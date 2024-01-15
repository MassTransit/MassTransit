namespace MassTransit.AmazonSqsTransport.Tests.Persistence
{
    using System.Threading.Tasks;


    public class EmptyConsumer : IConsumer<SimpleMessage>
    {
        public Task Consume(ConsumeContext<SimpleMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}
