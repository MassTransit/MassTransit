namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using DependencyInjection;


    public class ScopeConsumeFilter :
        IFilter<ConsumeContext>
    {
        readonly IConsumeScopeProvider _scopeProvider;

        public ScopeConsumeFilter(IConsumeScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            await using var scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scope");
        }
    }
}
