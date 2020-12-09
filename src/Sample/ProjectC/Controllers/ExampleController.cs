namespace ProjectC.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MassTransit;
    using MessageContract;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly ILogger<ExampleController> logger;
        private readonly IRequestClient<IMessageExample> messageExampleRequestClient;

        public ExampleController(
            IRequestClient<IMessageExample> messageExampleRequestClient,
            ILogger<ExampleController> logger)
        {
            this.messageExampleRequestClient = messageExampleRequestClient;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                var result = await messageExampleRequestClient.GetResponse<IMessageResult>(new {TestMessage = "Testing a test"});
                stopWatch.Stop();
                logger.LogInformation($"Finished within {stopWatch.Elapsed.Seconds} seconds");
                return result.Message.Done;
            }
            catch (Exception)
            {
                stopWatch.Stop();
                logger.LogInformation($"Finished within {stopWatch.Elapsed.Seconds} seconds");
                throw;
            }
        }
    }
}
