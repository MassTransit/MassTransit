using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using MassTransit.Transports.OnRamp;
using GreenPipes;
using MassTransit.EntityFrameworkCoreIntegration.OnRamp;
using Microsoft.EntityFrameworkCore;
using SampleApi.Controllers;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Data.SQLite;
using Npgsql;

namespace SampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                
                cfg.UsingRabbitMq((x, y) =>
                {
                    y.UseOutboxTransport(x);

                    y.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });

            services.AddMassTransitHostedService();

            // Using EFCore for interacting with db
            //services.AddOnRampTransport<OnRampDbContext>(cfg =>
            //{
            //    cfg.DisableServices = true;
            //});
            //services.AddDbContext<OnRampDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("mydb")));

            // ## For direct IDbConnection instead of EFCore
            //services.AddOnRampTransport(cfg =>
            //{
            //    cfg.DisableServices = true;
            //    cfg.PrefetchCount = 500;
            //});
            //services.AddScoped<DbConnection>(p => new SqlConnection(p.GetRequiredService<IConfiguration>().GetConnectionString("mydb")));

            // ## For sqlite
            services.AddOnRampTransport(cfg =>
            {
                cfg.DisableServices = true;
                cfg.PrefetchCount = 500;
                cfg.UsePostgres();
            });
            // Initializer
            //services.AddScoped<DbConnection>(p => new SQLiteConnection(p.GetRequiredService<IConfiguration>().GetConnectionString("mydb")));
            services.AddScoped<DbConnection>(p => new NpgsqlConnection(p.GetRequiredService<IConfiguration>().GetConnectionString("mydb")));

            services.AddOnRampTransportHostedService();

            services.AddOpenApiDocument(cfg => cfg.PostProcess = d => d.Info.Title = "Sample-Api");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
