namespace ProjectA
{
    using System;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;
    using MassTransit.ConsumeConfigurators;
    using MessageContract;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMassTransit(
                c =>
                {
                    c.AddConsumer<MessageExampleConsumer>();
                    c.UsingAzureServiceBus(this.ConfigureBus);
                });
            services.AddMassTransitHostedService();
        }

        private void ConfigureBus(IBusRegistrationContext context, IServiceBusBusFactoryConfigurator cfg)
        {
            static void Retry<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
                where TConsumer : class
            {
                configurator.UseMessageRetry(
                    _ => _.Exponential(
                        2,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(6),
                        TimeSpan.FromSeconds(2)));
            }

            cfg.Host(configuration["ServiceBusConnection"]);
            cfg.EnableDeadLetteringOnMessageExpiration = false;
            cfg.SubscriptionEndpoint<IMessageExample>(
                "example-sub",
                _ => _.ConfigureConsumer<MessageExampleConsumer>(context, Retry));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
