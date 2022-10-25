namespace MicrosoftDeployTopology;

using System;
using System.Threading;
using System.Threading.Tasks;
using ContainerConsumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.DeployTopologyOnly = true;

                cfg.ConfigureEndpoints(context);
            });
        });

        var provider = services.BuildServiceProvider();

        var busControl = provider.GetRequiredService<IBusControl>();

        try
        {
            await busControl.DeployAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token);

            Console.WriteLine("Topology Deployed");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to deploy topology: {0}", ex);
        }
    }
}
