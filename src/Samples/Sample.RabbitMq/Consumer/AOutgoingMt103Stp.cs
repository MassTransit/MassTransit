using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
    public class AOutgoingMt103Stp : IConsumer<IAOutgoingMt103Stp>
    {
        public Task Consume(ConsumeContext<IAOutgoingMt103Stp> context)
        {
            return context.RespondAsync<IAOutgoingMt103Stp>(new { });
        }
    }
}
