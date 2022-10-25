namespace SchedulingDelayed;

using MassTransit;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();

        services.AddMassTransit(x =>
        {
            x.AddDelayedMessageScheduler();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.UseDelayedMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
