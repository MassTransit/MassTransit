using MassTransit;
using SampleService.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Queue
{
    public class ASwiftIO : IConsumer<ASwiftIO>
    {
        public Task Consume(ConsumeContext<ASwiftIO> context)
        {
            return context.RespondAsync<ASwiftIO>(new { });
        }
    }
}
