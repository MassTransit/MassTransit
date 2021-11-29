namespace MassTransit.GrpcTransport.Tests.JobServiceTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;


    public class EncodeVideoConsumer :
        IJobConsumer<EncodeVideo>
    {
        readonly ILogger _logger;

        public EncodeVideoConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Run(JobContext<EncodeVideo> context)
        {
            _logger.LogInformation("Encoding Video: {VideoId} ({Path})", context.Job.VideoId, context.Job.Path);

            await Task.Delay(TimeSpan.FromSeconds(context.Job.Duration));
        }
    }
}
