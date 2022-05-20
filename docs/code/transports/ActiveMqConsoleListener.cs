namespace ActiveMqConsoleListener;

using System.Threading.Tasks;
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
                    x.UsingActiveMq((context, cfg) =>
                    {
                        cfg.Host("localhost", h =>
                        {
                            h.UseSsl();

                            h.Username("admin");
                            h.Password("admin");
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
