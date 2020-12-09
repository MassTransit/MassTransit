namespace ProjectA
{
    using System.Threading.Tasks;
    using MassTransit;
    using MessageContract;

    public class MessageExampleConsumer : IConsumer<IMessageExample>
    {
        public async Task Consume(ConsumeContext<IMessageExample> context)
        {
            await Task.Delay(4000);

            await context.RespondAsync<IMessageResult>(new{ Done = true });
        }
    }
}
