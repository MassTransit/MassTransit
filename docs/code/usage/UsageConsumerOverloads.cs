namespace UsageConsumerOverloads
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UsageConsumer;
    using UsageContracts;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("order-service", e =>
                {
                    // delegate consumer factory
                    e.Consumer(() => new SubmitOrderConsumer());

                    // another delegate consumer factory, with dependency
                    e.Consumer(() => new LogOrderSubmittedConsumer(Console.Out));

                    // a type-based factory that returns an object (specialized uses)
                    var consumerType = typeof(SubmitOrderConsumer);
                    e.Consumer(consumerType, type => Activator.CreateInstance(consumerType));
                });
            });
        }
    }

    class LogOrderSubmittedConsumer :
        IConsumer<OrderSubmitted>
    {
        readonly TextWriter _writer;

        public LogOrderSubmittedConsumer(TextWriter writer)
        {
            _writer = writer;
        }

        public async Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            await _writer.WriteLineAsync($"Order submitted: {context.Message.OrderId}");
        }
    }
}