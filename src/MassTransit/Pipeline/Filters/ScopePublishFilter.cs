namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Scoping;


    public class ScopePublishFilter<T> :
        IFilter<PublishContext<T>>
        where T : class
    {
        readonly IPublishScopeProvider _scopeProvider;

        public ScopePublishFilter(IPublishScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            using IPublishScopeContext<T> scope = _scopeProvider.GetScope(context);
            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
