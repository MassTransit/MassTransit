using System;
using System.Threading.Tasks;
using MassTransit;

namespace ConfigureBatchConsoleListener;

public class Program
{
    public static async Task Main()
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost", h =>
            {
                h.ConfigureBatchPublish(x =>
                {
                    x.Enabled = true;
                    x.Timeout = TimeSpan.FromMilliseconds(2);
                });
            });
        });
    }
}
