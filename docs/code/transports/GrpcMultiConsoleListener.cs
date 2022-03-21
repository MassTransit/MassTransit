namespace GrpcMultiConsoleListener;

using System;
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
                    x.UsingGrpc((context, cfg) =>
                    {
                        cfg.Host(h =>
                        {
                            h.Host = "127.0.0.1";
                            h.Port = 19796;

                            h.AddServer(new Uri("127.0.0.1:19797"));
                            h.AddServer(new Uri("127.0.0.1:19798"));
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
