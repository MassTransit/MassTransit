namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Scoping;


    public class ScopeSendFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly ISendScopeProvider _scopeProvider;

        public ScopeSendFilter(ISendScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            using ISendScopeContext<T> scope = _scopeProvider.GetScope(context);
            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
