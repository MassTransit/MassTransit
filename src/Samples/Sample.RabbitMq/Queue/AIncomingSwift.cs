using MassTransit;
using SampleService.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Queue
{
    public class AIncomingSwift : IConsumer<AIncomingSwift>
    {
        public Task Consume(ConsumeContext<AIncomingSwift> context)
        {
            return context.RespondAsync<AIncomingSwift>(new { });
        }
    }
}
