namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Scoping;


    public class ScopeMessageFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly IMessageScopeProvider _scopeProvider;

        public ScopeMessageFilter(IMessageScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            using var scope = _scopeProvider.GetScope(context);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
