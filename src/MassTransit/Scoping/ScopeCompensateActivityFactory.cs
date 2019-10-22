namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using Courier;
    using GreenPipes;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public class ScopeCompensateActivityFactory<TActivity, TArguments> :
        ICompensateActivityFactory<TActivity, TArguments>
        where TActivity : class, ICompensateActivity<TArguments>
        where TArguments : class
    {
        readonly ICompensateActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopeCompensateActivityFactory(ICompensateActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Compensate(CompensateContext<TArguments> context, IPipe<CompensateActivityContext<TActivity, TArguments>> next)
        {
            using (ICompensateActivityScopeContext<TActivity, TArguments> scope = _scopeProvider.GetScope(context))
            {
                await next.Send(scope.Context).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("scopeCompensateActivityFactory");

            _scopeProvider.Probe(scope);
        }
    }
}
