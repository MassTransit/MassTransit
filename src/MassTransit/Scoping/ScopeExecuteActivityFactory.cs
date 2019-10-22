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
    public class ScopeExecuteActivityFactory<TActivity, TArguments> :
        IExecuteActivityFactory<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityScopeProvider<TActivity, TArguments> _scopeProvider;

        public ScopeExecuteActivityFactory(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            using (IExecuteActivityScopeContext<TActivity, TArguments> scope = _scopeProvider.GetScope(context))
            {
                await next.Send(scope.Context).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("scopeExecuteActivityFactory");

            _scopeProvider.Probe(scope);
        }
    }
}
