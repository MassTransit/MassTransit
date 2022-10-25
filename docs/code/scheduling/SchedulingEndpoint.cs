namespace SchedulingEndpoint;

using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();

        Uri schedulerEndpoint = new Uri("queue:scheduler");

        services.AddMassTransit(x =>
        {
            x.AddMessageScheduler(schedulerEndpoint);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseMessageScheduler(schedulerEndpoint);

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
