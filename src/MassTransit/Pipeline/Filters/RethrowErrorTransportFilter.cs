namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Internals.Extensions;


    public class RethrowErrorTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        public async Task Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            await context.NotifyFaulted(context.Exception).ConfigureAwait(false);

            context.Exception.Rethrow();
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("log-fault");
        }
    }
}
