namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class FaultDeadLetterFilter :
        IFilter<ReceiveContext>
    {
        public Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            throw new MessageNotConsumedException(context.InputAddress, "The message was not consumed");
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("fault-not-consumed");
        }
    }
}
