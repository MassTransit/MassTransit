namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Scoping;


    public class ScopeConsumeFilter :
        IFilter<ConsumeContext>
    {
        readonly IConsumerScopeProvider _scopeProvider;

        public ScopeConsumeFilter(IConsumerScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            using (var scope = _scopeProvider.GetScope(context))
            {
                await next.Send(scope.Context).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
