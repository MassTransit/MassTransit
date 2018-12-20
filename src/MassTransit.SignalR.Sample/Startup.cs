using System;
using MassTransit.RabbitMqTransport;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Sample.HostedServices;
using MassTransit.SignalR.Sample.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransit.SignalR.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddMassTransitBackplane(out var hubConsumers, typeof(Startup).Assembly);

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.CreateBackplaneEndpoints<IRabbitMqReceiveEndpointConfigurator>(provider, host, hubConsumers, e =>
                {
                    e.AutoDelete = true;
                    e.Durable = false;
                });
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddSingleton<IHostedService, BusService>();

            services.AddSingleton(provider => provider.GetRequiredService<IBus>().CreateRequestClient<GroupManagement<ChatHub>>(TimeSpan.FromSeconds(30)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chat");
            });
        }
    }
}
