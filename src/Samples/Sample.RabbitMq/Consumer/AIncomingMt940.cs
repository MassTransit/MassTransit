using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
    public class AIncomingMt940 : IConsumer<IAIncomingMt940>
    {
        public Task Consume(ConsumeContext<IAIncomingMt940> context)
        {
            return context.RespondAsync<IAIncomingMt940>(new { });
        }
    }
}
