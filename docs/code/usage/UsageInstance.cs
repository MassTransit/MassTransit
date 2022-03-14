namespace UsageInstance
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using UsageConsumer;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var submitOrderConsumer = new SubmitOrderConsumer();

            await using var provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("order-service", e =>
                        {
                            e.Instance(submitOrderConsumer);
                        });
                    });
                })
                .BuildServiceProvider();
        }
    }
}
