using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
    public class AOutgoingMt940 : IConsumer<IAOutgoingMt940>
    {
        public Task Consume(ConsumeContext<IAOutgoingMt940> context)
        {
            return context.RespondAsync<IAOutgoingMt940>(new { });
        }
    }
}
