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
    using System.Threading.Tasks;
    using JobSystem.Jobs;
    using MassTransit;
    using MassTransit.Conductor.Configuration;
    using MassTransit.JobService;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var options = new ServiceInstanceOptions()
                    .EnableInstanceEndpoint();

                cfg.ServiceInstance(options, instance =>
                {
                    instance.ConfigureJobServiceEndpoints();

                    var queueName = instance.EndpointNameFormatter.Consumer<ConvertVideoJobConsumer>();

                    instance.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer(() => new ConvertVideoJobConsumer());
                    });
                });
            });
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
