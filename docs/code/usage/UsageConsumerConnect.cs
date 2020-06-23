namespace UsageConsumerConnect
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

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                var handle = busControl.ConnectConsumer<OrderAcknowledgedConsumer>();

                var endpoint = await busControl.GetSendEndpoint(new Uri("queue:order-service"));

                await endpoint.Send<SubmitOrder>(new 
                    {
                        OrderId = InVar.Id,
                        __ResponseAddress = busControl.Address
                    });

                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());

                // disconnect the consumer from the bus endpoint
                handle.Disconnect();
            }
            finally
            {
                await busControl.StopAsync();
            }
        }            
    }

    class OrderAcknowledgedConsumer :
        IConsumer<SubmitOrderAcknowledged>
    {
        public async Task Consume(ConsumeContext<SubmitOrderAcknowledged> context)
        {
        }
    }
}