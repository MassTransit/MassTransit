namespace UsageInstance;

using System.Threading.Tasks;
using UsageConsumer;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        var submitOrderConsumer = new SubmitOrderConsumer();

        await Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("order-service", e =>
                        {
                            e.Instance(submitOrderConsumer);
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
