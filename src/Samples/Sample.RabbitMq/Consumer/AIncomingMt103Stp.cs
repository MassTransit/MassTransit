using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
    public class AIncomingMt103Stp : IConsumer<IAIncomingMt103Stp>
    {
        public Task Consume(ConsumeContext<IAIncomingMt103Stp> context)
        {
            return context.RespondAsync<IAIncomingMt103Stp>(new { });
        }
    }
}
