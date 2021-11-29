namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Courier;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class ScopeCompensateActivityFactory<TActivity, TLog> :
        ICompensateActivityFactory<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly ICompensateActivityScopeProvider<TActivity, TLog> _scopeProvider;

        public ScopeCompensateActivityFactory(ICompensateActivityScopeProvider<TActivity, TLog> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            await using ICompensateActivityScopeContext<TActivity, TLog> scope = await _scopeProvider.GetActivityScope(context).ConfigureAwait(false);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("scopeCompensateActivityFactory");

            _scopeProvider.Probe(scope);
        }
    }
}
