using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MassTransit.SignalR.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            CreateWebHostBuilder(args, configuration).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
    }
}
