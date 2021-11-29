namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using DependencyInjection;


    public class ScopeMessageFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly IConsumeScopeProvider _scopeProvider;

        public ScopeMessageFilter(IConsumeScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            await using IConsumeScopeContext<T> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
