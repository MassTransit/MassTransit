namespace UsageMessageCorrelation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UsageContracts;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
            try
            {
                var endpoint = await busControl.GetSendEndpoint(new Uri("queue:order-service"));

                // Set CorrelationId using SendContext<T>
                await endpoint.Send<SubmitOrder>(new { OrderId = InVar.Id }, context =>
                    context.CorrelationId = context.Message.OrderId);

                // Set CorrelationId using initializer header
                await endpoint.Send<SubmitOrder>(new
                { 
                    OrderId = InVar.Id,
                    __CorrelationId = InVar.Id

                    // InVar.Id returns the same value within the message initializer
                });
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}