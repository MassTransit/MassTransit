namespace MicrosoftContainer
{
    using System.Threading.Tasks;
    using ContainerConsumers;
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
                        x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));

                        x.SetKebabCaseEndpointNameFormatter();

                        x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                    });
                })
                .Build()
                .RunAsync();
        }
    }
}
