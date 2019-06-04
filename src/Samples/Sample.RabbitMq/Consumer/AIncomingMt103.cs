using MassTransit;
using SampleService.Contracts;
using System.Threading.Tasks;

namespace SampleService.Consumer
{
   public class AIncomingMt103 : IConsumer<IAIncomingMt103>
    {
        public Task Consume(ConsumeContext<IAIncomingMt103> context)
        {
            return context.RespondAsync<IAIncomingMt103>(new { });
        }
    }
}
