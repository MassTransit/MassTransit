namespace AspNetCorePublisher
{
    using System;
    using System.Threading.Tasks;
    using EventContracts;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
            });

            services.AddMassTransitHostedService();
        }
    }
}
