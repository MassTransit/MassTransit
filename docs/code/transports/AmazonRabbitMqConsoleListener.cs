namespace AmazonRabbitMqConsoleListener;

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
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(new Uri("amqps://b-12345678-1234-1234-1234-123456789012.mq.us-east-2.amazonaws.com:5671"), h =>
                        {
                            h.Username("username");
                            h.Password("password");
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
