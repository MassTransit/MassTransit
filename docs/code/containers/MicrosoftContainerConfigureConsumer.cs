namespace MicrosoftContainerConfigureConsumer;

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

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint("order-service", e =>
                {
                    e.ConfigureConsumer<SubmitOrderConsumer>(context);
                });
            });
        });
    }
}
