namespace MicrosoftContainerFormatterInline
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Definition;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance);
                });
            });
        }
    }
}