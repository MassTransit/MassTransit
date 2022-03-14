namespace UsageConsumerTemporaryEndpoint
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using UsageConsumer;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(), e =>
                        {
                            e.ConfigureConsumer<SubmitOrderConsumer>(context);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider();

        }
    }
}
