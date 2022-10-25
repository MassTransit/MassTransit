namespace MicrosoftConnect;

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
                cfg.ConfigureEndpoints(context, x => x.Exclude<SubmitOrderConsumer>()));
        });

        var provider = services.BuildServiceProvider();

        var busControl = provider.GetRequiredService<IBusControl>();

        await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
        try
        {
            var connector = provider.GetRequiredService<IReceiveEndpointConnector>();

            var handle = connector.ConnectReceiveEndpoint("order-queue", (context,cfg) =>
            {
                cfg.ConfigureConsumer<SubmitOrderConsumer>(context);
            });

            await handle.Ready;

            Console.WriteLine("Press enter to exit");

            await Task.Run(() => Console.ReadLine());
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}
