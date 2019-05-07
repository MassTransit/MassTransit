namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using Courier;
    using GreenPipes;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Util;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public class ScopeCompensateActivityFactory<TActivity, TArguments> :
        CompensateActivityFactory<TActivity, TArguments>
        where TActivity : class, CompensateActivity<TArguments>
        where TArguments : class
    {
        static readonly ILogger _logger = Logger.Get<ScopeCompensateActivityFactory<TActivity, TArguments>>();

        readonly ICompensateActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopeCompensateActivityFactory(ICompensateActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TArguments> context,
            IRequestPipe<CompensateActivityContext<TActivity, TArguments>, CompensationResult> next)
        {
            using (ICompensateActivityScopeContext<TActivity, TArguments> scope = _scopeProvider.GetScope(context))
            {
                _logger.LogDebug("CompensateActivityFactory: Compensating: {0}", TypeMetadataCache<TActivity>.ShortName);

                return await next.Send(scope.Context).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("scopeCompensateActivityFactory");

            _scopeProvider.Probe(scope);
        }
    }
}
