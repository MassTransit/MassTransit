namespace ProjectC
{
    using System;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;
    using MessageContract;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMassTransit(
                c =>
                {
                    int timeoutOffset = 0; // Workaround: With additional 30 sec it works fine, Because ExpireAt is not calculated correctly?
                    RequestTimeout requestTimeout = RequestTimeout.After(0, 0, 0, timeoutOffset + 10);
                    c.AddRequestClient<IMessageExample>(requestTimeout);
                    c.UsingAzureServiceBus(this.ConfigureBus);
                });
            services.AddMassTransitHostedService();
        }

        private void ConfigureBus(IBusRegistrationContext context, IServiceBusBusFactoryConfigurator cfg)
        {
            cfg.UseMessageRetry(
                _ => _.Exponential(
                    2,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(2)));

            cfg.Host(Configuration["ServiceBusConnection"]);
            cfg.EnableDeadLetteringOnMessageExpiration = false;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
