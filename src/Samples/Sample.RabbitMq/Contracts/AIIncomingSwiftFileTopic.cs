using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Contracts
{
    public interface AIIncomingSwiftFileTopic
    {
        Task Consume(ConsumeContext<AIIncomingSwiftFileTopic> context);
    }
}
