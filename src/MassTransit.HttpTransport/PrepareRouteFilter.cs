namespace MassTransit.HttpTransport
{
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Hosting;
    using MassTransit.Pipeline;


    public class PrepareRouteFilter : IFilter<OwinHostContext>
    {
        public PrepareRouteFilter(ReceiveSettings settings)
        {
            
        }

        public void Probe(ProbeContext context)
        {
            //no-op
        }

        public Task Send(OwinHostContext context, IPipe<OwinHostContext> next)
        {
            //TODO: set up routes here?
            return Task.FromResult(true);
        }
    }
}