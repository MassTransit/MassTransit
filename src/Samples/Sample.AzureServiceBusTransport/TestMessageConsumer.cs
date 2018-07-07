namespace Sample.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class TestMessageConsumer : IConsumer<TestMessage>
    {
        public Task Consume(ConsumeContext<TestMessage> context)
        {
            Console.WriteLine(context.Message.Name);

            return Task.CompletedTask;
        }
    }
}