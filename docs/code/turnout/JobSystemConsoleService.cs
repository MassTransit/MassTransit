namespace JobSystem.Jobs
{
    using System;

    public interface ConvertVideo
    {
        Guid VideoId { get; }
        string Format { get; }
    }
}

namespace JobSystemConsoleService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSystem.Jobs;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ConvertVideoJobConsumer>(cfg =>
                {
                    cfg.Options<JobOptions<ConvertVideo>>(options => options
                        .SetJobTimeout(TimeSpan.FromMinutes(15))
                        .SetConcurrentJobLimit(10));
                });

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ServiceInstance(instance =>
                    {
                        instance.ConfigureJobServiceEndpoints();

                        instance.ConfigureEndpoints(context);
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        public class ConvertVideoJobConsumer :
            IJobConsumer<ConvertVideo>
        {
            public async Task Run(JobContext<ConvertVideo> context)
            {
                // simulate converting the video
                await Task.Delay(TimeSpan.FromMinutes(3));
            }
        }
    }
}
