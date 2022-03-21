namespace ServiceBusConsoleListener;

using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host("connection-string");
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
