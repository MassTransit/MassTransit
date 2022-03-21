namespace UsageConsumerBus;

using System.Threading.Tasks;
using UsageConsumer;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
