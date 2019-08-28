namespace MassTransit.Containers.Tests.Scenarios
{
    using System.Threading.Tasks;
    using TestFramework.Messages;


    public class PingRequestConsumer :
        IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
        }
    }
}
