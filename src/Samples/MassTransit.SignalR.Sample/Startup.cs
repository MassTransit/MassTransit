// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.SignalR.Sample
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using HostedServices;
    using Hubs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using RabbitMqTransport;
    using Scoping;


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddMassTransitBackplane(out IReadOnlyDictionary<Type, IReadOnlyList<Type>> hubConsumers, typeof(Startup).Assembly);

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

            services.AddScoped<ScopedConsumeContextProvider>();
            services.AddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext());

            services.AddScoped(provider => (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ??
                provider.GetRequiredService<IBus>());

            services.AddScoped(provider => (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ??
                provider.GetRequiredService<IBus>());

            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBus>());
            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

            services.AddSingleton<IHostedService, BusService>();

            services.AddSingleton(provider => provider.GetRequiredService<IBus>().CreateRequestClient<GroupManagement<ChatHub>>(TimeSpan.FromSeconds(30)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseFileServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chat");
            });
        }
    }
}
