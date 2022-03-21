namespace UsageConsumerTemporaryEndpoint;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
                        cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(), e =>
                        {
                            e.ConfigureConsumer<SubmitOrderConsumer>(context);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
