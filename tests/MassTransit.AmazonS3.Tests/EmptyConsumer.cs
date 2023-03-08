namespace MassTransit.AmazonS3.Tests
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
