using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
   public class AOutgoingMt103 : IConsumer<IAOutgoingMt103>
    {
        public Task Consume(ConsumeContext<IAOutgoingMt103> context)
        {
            return context.RespondAsync<IAOutgoingMt103>(new { });
        }
    }
}
